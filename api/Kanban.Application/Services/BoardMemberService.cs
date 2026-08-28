using Kanban.Application.Common;
using Kanban.Application.DTOs.Members;
using Kanban.Application.Interfaces;
using Kanban.Domain.Entities;

namespace Kanban.Application.Services;

public class BoardMemberService
{
    private readonly IBoardRepository _boards;
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;
    private readonly IBoardNotifier _notifier;

    public BoardMemberService(IBoardRepository boards, IUserRepository users, IUnitOfWork uow, IBoardNotifier notifier)
    {
        _boards = boards;
        _users = users;
        _uow = uow;
        _notifier = notifier;
    }

    public async Task<Result<List<BoardMemberResponse>>> GetMembersAsync(Guid boardId, Guid userId, CancellationToken ct = default)
    {
        var membership = await _boards.GetMembershipAsync(boardId, userId, ct);
        if (membership is null)
            return Result.Failure<List<BoardMemberResponse>>("Tablero no encontrado.", notFound: true);

        var members = await _boards.GetMembersAsync(boardId, ct);
        return Result.Success(members.Select(m => ToResponse(m)).ToList());
    }

    /// <summary>Solo el Owner puede invitar — evita que un Editor le dé acceso al tablero a cualquiera.</summary>
    public async Task<Result<BoardMemberResponse>> InviteAsync(Guid boardId, Guid requestingUserId, InviteMemberRequest request, CancellationToken ct = default)
    {
        var requesterMembership = await _boards.GetMembershipAsync(boardId, requestingUserId, ct);
        if (requesterMembership is null)
            return Result.Failure<BoardMemberResponse>("Tablero no encontrado.", notFound: true);
        if (requesterMembership.Role != BoardRole.Owner)
            return Result.Failure<BoardMemberResponse>("Solo el dueño puede invitar miembros.", forbidden: true);

        if (!Enum.TryParse<BoardRole>(request.Role, ignoreCase: true, out var role) || role == BoardRole.Owner)
            return Result.Failure<BoardMemberResponse>("Rol inválido. Usá 'Editor' o 'Viewer'.");

        var invitedUser = await _users.GetByEmailAsync(EmailNormalizer.Normalize(request.Email), ct);
        if (invitedUser is null)
            return Result.Failure<BoardMemberResponse>("No existe ninguna cuenta con ese email.");

        var existingMembership = await _boards.GetMembershipAsync(boardId, invitedUser.Id, ct);
        if (existingMembership is not null)
            return Result.Failure<BoardMemberResponse>("Ese usuario ya es miembro de este tablero.");

        var member = BoardMember.Create(boardId, invitedUser.Id, role);
        await _boards.AddMemberAsync(member, ct);
        await _uow.SaveChangesAsync(ct);

        return Result.Success(ToResponse(member, invitedUser));
    }

    /// <summary>El dueño no puede sacarse a sí mismo ni a otro Owner: un tablero siempre necesita uno.</summary>
    public async Task<Result> RemoveAsync(Guid boardId, Guid requestingUserId, Guid targetUserId, CancellationToken ct = default)
    {
        var requesterMembership = await _boards.GetMembershipAsync(boardId, requestingUserId, ct);
        if (requesterMembership is null)
            return Result.Failure("Tablero no encontrado.", notFound: true);
        if (requesterMembership.Role != BoardRole.Owner)
            return Result.Failure("Solo el dueño puede eliminar miembros.", forbidden: true);

        var targetMembership = await _boards.GetMembershipAsync(boardId, targetUserId, ct);
        if (targetMembership is null)
            return Result.Failure("Ese usuario no es miembro de este tablero.", notFound: true);
        if (targetMembership.Role == BoardRole.Owner)
            return Result.Failure("No se puede eliminar al dueño del tablero.");

        _boards.RemoveMember(targetMembership);
        await _uow.SaveChangesAsync(ct);
        await _notifier.MemberRemovedAsync(boardId, targetUserId, ct);

        return Result.Success();
    }

    private static BoardMemberResponse ToResponse(BoardMember member, User? user = null)
    {
        var u = user ?? member.User;
        return new(member.UserId, u.Name, u.Email, member.Role.ToString(), member.CreatedAt);
    }
}

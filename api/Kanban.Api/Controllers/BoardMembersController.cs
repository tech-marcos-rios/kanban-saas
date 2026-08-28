using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Kanban.Application.DTOs.Members;
using Kanban.Application.Services;

namespace Kanban.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/boards/{boardId:guid}/members")]
public class BoardMembersController : ApiControllerBase
{
    private readonly BoardMemberService _memberService;

    public BoardMembersController(BoardMemberService memberService) => _memberService = memberService;

    [HttpGet]
    [ProducesResponseType(typeof(List<BoardMemberResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(Guid boardId, CancellationToken ct)
    {
        var result = await _memberService.GetMembersAsync(boardId, CurrentUserId, ct);
        if (result.IsFailure)
            return ToErrorResult(result.Error!, result.IsNotFound, result.IsForbidden);

        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BoardMemberResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Invite(Guid boardId, [FromBody] InviteMemberRequest request, CancellationToken ct)
    {
        var result = await _memberService.InviteAsync(boardId, CurrentUserId, request, ct);
        if (result.IsFailure)
            return ToErrorResult(result.Error!, result.IsNotFound, result.IsForbidden);

        return CreatedAtAction(nameof(GetAll), new { boardId }, result.Value);
    }

    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remove(Guid boardId, Guid userId, CancellationToken ct)
    {
        var result = await _memberService.RemoveAsync(boardId, CurrentUserId, userId, ct);
        if (result.IsFailure)
            return ToErrorResult(result.Error!, result.IsNotFound, result.IsForbidden);

        return NoContent();
    }
}

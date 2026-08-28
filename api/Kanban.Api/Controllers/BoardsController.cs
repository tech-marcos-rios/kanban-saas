using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Kanban.Application.DTOs.Boards;
using Kanban.Application.Services;

namespace Kanban.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/boards")]
public class BoardsController : ControllerBase
{
    private readonly BoardService _boardService;

    public BoardsController(BoardService boardService) => _boardService = boardService;

    [HttpPost]
    [ProducesResponseType(typeof(BoardResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateBoardRequest request, CancellationToken ct)
    {
        var result = await _boardService.CreateAsync(CurrentUserId, request, ct);
        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<BoardResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyBoards(CancellationToken ct)
    {
        var result = await _boardService.GetMyBoardsAsync(CurrentUserId, ct);
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BoardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _boardService.GetByIdAsync(id, CurrentUserId, ct);
        return result.IsFailure ? NotFound(new { error = result.Error }) : Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BoardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Rename(Guid id, [FromBody] UpdateBoardRequest request, CancellationToken ct)
    {
        var result = await _boardService.RenameAsync(id, CurrentUserId, request, ct);
        if (result.IsFailure)
            return ToErrorResult(result.Error!, result.IsNotFound, result.IsForbidden);

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _boardService.DeleteAsync(id, CurrentUserId, ct);
        if (result.IsFailure)
            return ToErrorResult(result.Error!, result.IsNotFound, result.IsForbidden);

        return NoContent();
    }

    private IActionResult ToErrorResult(string error, bool notFound, bool forbidden)
    {
        if (notFound) return NotFound(new { error });
        if (forbidden) return StatusCode(StatusCodes.Status403Forbidden, new { error });
        return BadRequest(new { error });
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")
        ?? throw new InvalidOperationException());
}

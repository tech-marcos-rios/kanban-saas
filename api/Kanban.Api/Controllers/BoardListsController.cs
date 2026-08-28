using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Kanban.Application.DTOs.Lists;
using Kanban.Application.Services;

namespace Kanban.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/boards/{boardId:guid}/lists")]
public class BoardListsController : ApiControllerBase
{
    private readonly BoardListService _listService;

    public BoardListsController(BoardListService listService) => _listService = listService;

    [HttpGet]
    [ProducesResponseType(typeof(List<BoardListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(Guid boardId, CancellationToken ct)
    {
        var result = await _listService.GetForBoardAsync(boardId, CurrentUserId, ct);
        if (result.IsFailure)
            return ToErrorResult(result.Error!, result.IsNotFound, result.IsForbidden);

        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BoardListResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(Guid boardId, [FromBody] CreateBoardListRequest request, CancellationToken ct)
    {
        var result = await _listService.CreateAsync(boardId, CurrentUserId, request, ct);
        if (result.IsFailure)
            return ToErrorResult(result.Error!, result.IsNotFound, result.IsForbidden);

        return CreatedAtAction(nameof(GetAll), new { boardId }, result.Value);
    }

    [HttpPut("{listId:guid}")]
    [ProducesResponseType(typeof(BoardListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Rename(Guid boardId, Guid listId, [FromBody] UpdateBoardListRequest request, CancellationToken ct)
    {
        var result = await _listService.RenameAsync(boardId, listId, CurrentUserId, request, ct);
        if (result.IsFailure)
            return ToErrorResult(result.Error!, result.IsNotFound, result.IsForbidden);

        return Ok(result.Value);
    }

    [HttpPut("{listId:guid}/position")]
    [ProducesResponseType(typeof(List<BoardListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Move(Guid boardId, Guid listId, [FromBody] MoveBoardListRequest request, CancellationToken ct)
    {
        var result = await _listService.MoveAsync(boardId, listId, CurrentUserId, request.Position, ct);
        if (result.IsFailure)
            return ToErrorResult(result.Error!, result.IsNotFound, result.IsForbidden);

        return Ok(result.Value);
    }

    [HttpDelete("{listId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid boardId, Guid listId, CancellationToken ct)
    {
        var result = await _listService.DeleteAsync(boardId, listId, CurrentUserId, ct);
        if (result.IsFailure)
            return ToErrorResult(result.Error!, result.IsNotFound, result.IsForbidden);

        return NoContent();
    }
}

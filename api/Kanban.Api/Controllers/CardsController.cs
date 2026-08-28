using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Kanban.Application.DTOs.Cards;
using Kanban.Application.Services;

namespace Kanban.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/boards/{boardId:guid}/cards")]
public class CardsController : ApiControllerBase
{
    private readonly CardService _cardService;

    public CardsController(CardService cardService) => _cardService = cardService;

    [HttpGet("lists/{listId:guid}")]
    [ProducesResponseType(typeof(List<CardResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByList(Guid boardId, Guid listId, CancellationToken ct)
    {
        var result = await _cardService.GetByListAsync(boardId, listId, CurrentUserId, ct);
        if (result.IsFailure)
            return ToErrorResult(result.Error!, result.IsNotFound, result.IsForbidden);

        return Ok(result.Value);
    }

    [HttpPost("lists/{listId:guid}")]
    [ProducesResponseType(typeof(CardResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(Guid boardId, Guid listId, [FromBody] CreateCardRequest request, CancellationToken ct)
    {
        var result = await _cardService.CreateAsync(boardId, listId, CurrentUserId, request, ct);
        if (result.IsFailure)
            return ToErrorResult(result.Error!, result.IsNotFound, result.IsForbidden);

        return CreatedAtAction(nameof(GetById), new { boardId, cardId = result.Value!.Id }, result.Value);
    }

    [HttpGet("{cardId:guid}")]
    [ProducesResponseType(typeof(CardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid boardId, Guid cardId, CancellationToken ct)
    {
        var result = await _cardService.GetByIdAsync(boardId, cardId, CurrentUserId, ct);
        if (result.IsFailure)
            return ToErrorResult(result.Error!, result.IsNotFound, result.IsForbidden);

        return Ok(result.Value);
    }

    [HttpPut("{cardId:guid}")]
    [ProducesResponseType(typeof(CardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid boardId, Guid cardId, [FromBody] UpdateCardRequest request, CancellationToken ct)
    {
        var result = await _cardService.UpdateAsync(boardId, cardId, CurrentUserId, request, ct);
        if (result.IsFailure)
            return ToErrorResult(result.Error!, result.IsNotFound, result.IsForbidden);

        return Ok(result.Value);
    }

    [HttpPut("{cardId:guid}/assign")]
    [ProducesResponseType(typeof(CardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Assign(Guid boardId, Guid cardId, [FromBody] AssignCardRequest request, CancellationToken ct)
    {
        var result = await _cardService.AssignAsync(boardId, cardId, CurrentUserId, request, ct);
        if (result.IsFailure)
            return ToErrorResult(result.Error!, result.IsNotFound, result.IsForbidden);

        return Ok(result.Value);
    }

    [HttpPut("{cardId:guid}/move")]
    [ProducesResponseType(typeof(CardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Move(Guid boardId, Guid cardId, [FromBody] MoveCardRequest request, CancellationToken ct)
    {
        var result = await _cardService.MoveAsync(boardId, cardId, CurrentUserId, request, ct);
        if (result.IsFailure)
            return ToErrorResult(result.Error!, result.IsNotFound, result.IsForbidden);

        return Ok(result.Value);
    }

    [HttpDelete("{cardId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid boardId, Guid cardId, CancellationToken ct)
    {
        var result = await _cardService.DeleteAsync(boardId, cardId, CurrentUserId, ct);
        if (result.IsFailure)
            return ToErrorResult(result.Error!, result.IsNotFound, result.IsForbidden);

        return NoContent();
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.Api.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")
        ?? throw new InvalidOperationException());

    protected IActionResult ToErrorResult(string error, bool notFound, bool forbidden)
    {
        if (notFound) return NotFound(new { error });
        if (forbidden) return StatusCode(StatusCodes.Status403Forbidden, new { error });
        return BadRequest(new { error });
    }
}

using Microsoft.AspNetCore.Mvc;
using PromptOptimizer.Application.DTOs;
using PromptOptimizer.Application.Interfaces;

namespace PromptOptimizer.Api.Controllers;

/// <summary>
/// Endpoints for optimizing prompts.
/// </summary>
[ApiController]
[Route("api/prompt")]
[Produces("application/json")]
public class PromptController : ControllerBase
{
    private readonly IPromptOptimizationService _optimizationService;

    public PromptController(IPromptOptimizationService optimizationService)
    {
        _optimizationService = optimizationService;
    }

    /// <summary>
    /// Optimizes a user-provided prompt for the requested optimization style.
    /// </summary>
    [HttpPost("optimize")]
    [ProducesResponseType(typeof(OptimizePromptResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OptimizePromptResponse>> Optimize(
        [FromBody] OptimizePromptRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User?.Identity?.IsAuthenticated == true
            ? User.FindFirst("oid")?.Value ?? User.FindFirst("sub")?.Value
            : null;

        var response = await _optimizationService.OptimizeAsync(request, userId, cancellationToken);
        return Ok(response);
    }
}

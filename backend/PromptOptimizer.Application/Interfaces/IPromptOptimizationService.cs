using PromptOptimizer.Application.DTOs;

namespace PromptOptimizer.Application.Interfaces;

/// <summary>
/// Orchestrates prompt optimization: builds the system prompt, calls the AI
/// provider, scores the result and persists it.
/// </summary>
public interface IPromptOptimizationService
{
    Task<OptimizePromptResponse> OptimizeAsync(
        OptimizePromptRequest request,
        string? userId = null,
        CancellationToken cancellationToken = default);
}

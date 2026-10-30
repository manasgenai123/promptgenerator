using PromptOptimizer.Application.DTOs;

namespace PromptOptimizer.Application.Interfaces;

/// <summary>
/// Abstraction over the underlying LLM provider (OpenAI / Azure OpenAI).
/// </summary>
public interface IOpenAIService
{
    /// <summary>
    /// Sends the system + user prompt to the model and returns the structured result.
    /// </summary>
    Task<PromptOptimizationResult> GetOptimizedPromptAsync(
        string systemPrompt,
        string userPrompt,
        CancellationToken cancellationToken = default);
}

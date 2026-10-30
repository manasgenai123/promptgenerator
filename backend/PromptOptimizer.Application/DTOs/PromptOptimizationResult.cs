namespace PromptOptimizer.Application.DTOs;

/// <summary>
/// Raw optimization result returned by the AI provider abstraction.
/// </summary>
public class PromptOptimizationResult
{
    public string OptimizedPrompt { get; set; } = string.Empty;

    public IReadOnlyList<string> Improvements { get; set; } = new List<string>();
}

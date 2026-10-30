namespace PromptOptimizer.Application.DTOs;

/// <summary>
/// Response payload for POST /api/prompt/optimize.
/// </summary>
public class OptimizePromptResponse
{
    public string OriginalPrompt { get; set; } = string.Empty;

    public string OptimizedPrompt { get; set; } = string.Empty;

    public IReadOnlyList<string> Improvements { get; set; } = new List<string>();

    /// <summary>Overall prompt quality score (0-100) for the optimized prompt.</summary>
    public int QualityScore { get; set; }

    /// <summary>Approximate token count of the original prompt.</summary>
    public int OriginalTokenEstimate { get; set; }

    /// <summary>Approximate token count of the optimized prompt.</summary>
    public int OptimizedTokenEstimate { get; set; }

    public string OptimizationType { get; set; } = string.Empty;
}

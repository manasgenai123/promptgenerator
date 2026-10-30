using PromptOptimizer.Domain.Enums;

namespace PromptOptimizer.Domain.Entities;

/// <summary>
/// Persisted record of a single prompt optimization request/response.
/// </summary>
public class PromptOptimization
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string OriginalPrompt { get; set; } = string.Empty;

    public string OptimizedPrompt { get; set; } = string.Empty;

    public OptimizationType OptimizationType { get; set; }

    /// <summary>Improvement bullet points serialized as a single string (one per line).</summary>
    public string Improvements { get; set; } = string.Empty;

    public int QualityScore { get; set; }

    public int OriginalTokenEstimate { get; set; }

    public int OptimizedTokenEstimate { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Optional authenticated user id (Entra ID object id).</summary>
    public string? UserId { get; set; }
}

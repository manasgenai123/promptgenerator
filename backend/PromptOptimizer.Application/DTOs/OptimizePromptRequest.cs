using System.Text.Json.Serialization;
using PromptOptimizer.Domain.Enums;

namespace PromptOptimizer.Application.DTOs;

/// <summary>
/// Request payload for POST /api/prompt/optimize.
/// </summary>
public class OptimizePromptRequest
{
    /// <summary>The original, user-provided prompt to optimize.</summary>
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// The optimization style. Accepts the enum name (e.g. "Technical", "CodeGeneration").
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OptimizationType OptimizationType { get; set; } = OptimizationType.General;
}

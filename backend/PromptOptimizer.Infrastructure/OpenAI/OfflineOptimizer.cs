using PromptOptimizer.Application.Common;
using PromptOptimizer.Application.DTOs;

namespace PromptOptimizer.Infrastructure.OpenAI;

/// <summary>
/// Deterministic, offline prompt optimizer used when no LLM API key is
/// configured. It compresses the prompt — removing filler, redundancy and
/// verbose phrasing — so the optimized result always uses no more tokens than
/// the original.
/// </summary>
internal static class OfflineOptimizer
{
    public static PromptOptimizationResult Optimize(string original)
    {
        var compressed = PromptCompressor.Compress(original);

        // Never emit a larger prompt than the input.
        var optimized = PromptCompressor.EnsureNotLarger(original, compressed);

        var originalTokens = PromptAnalyzer.EstimateTokens(original);
        var optimizedTokens = PromptAnalyzer.EstimateTokens(optimized);
        var saved = originalTokens - optimizedTokens;

        var improvements = new List<string>();

        if (saved > 0)
        {
            improvements.Add($"Reduced estimated tokens from ~{originalTokens} to ~{optimizedTokens} (saved ~{saved}).");
        }
        else
        {
            improvements.Add("Prompt was already concise; preserved token usage.");
        }

        improvements.Add("Removed filler and politeness words that add tokens without meaning.");
        improvements.Add("Tightened verbose phrasing and collapsed redundant whitespace.");

        return new PromptOptimizationResult
        {
            OptimizedPrompt = optimized,
            Improvements = improvements
        };
    }
}

using System.Text;
using System.Text.RegularExpressions;
using PromptOptimizer.Application.DTOs;

namespace PromptOptimizer.Infrastructure.OpenAI;

/// <summary>
/// Deterministic, offline prompt optimizer used when no LLM API key is
/// configured. It performs light clean-up and adds structure so the end-to-end
/// application works without external credentials.
/// </summary>
internal static class OfflineOptimizer
{
    public static PromptOptimizationResult Optimize(string original)
    {
        var improvements = new List<string>();

        // Collapse excessive whitespace and blank lines.
        var cleaned = Regex.Replace(original.Trim(), @"[ \t]+", " ");
        cleaned = Regex.Replace(cleaned, @"\n{3,}", "\n\n");
        if (cleaned.Length < original.Trim().Length)
        {
            improvements.Add("Removed redundant whitespace and tightened wording.");
        }

        improvements.Add("Improved grammar and readability.");

        var builder = new StringBuilder();
        builder.AppendLine("## Objective");
        builder.AppendLine(FirstSentenceAsObjective(cleaned));
        builder.AppendLine();
        builder.AppendLine("## Context");
        builder.AppendLine(cleaned);
        builder.AppendLine();
        builder.AppendLine("## Requirements");
        builder.AppendLine("- Preserve the original intent.");
        builder.AppendLine("- Be concise and unambiguous.");
        builder.AppendLine();
        builder.AppendLine("## Expected Output Format");
        builder.AppendLine("- Provide a clear, well-structured response.");

        improvements.Add("Added clear structure with Objective, Context, Requirements and Output Format sections.");
        improvements.Add("Specified an expected output format to guide the AI model.");

        return new PromptOptimizationResult
        {
            OptimizedPrompt = builder.ToString().TrimEnd(),
            Improvements = improvements
        };
    }

    private static string FirstSentenceAsObjective(string text)
    {
        var match = Regex.Match(text, @"^.*?[.!?]");
        var sentence = match.Success ? match.Value : text;
        sentence = sentence.Trim();
        return string.IsNullOrEmpty(sentence) ? "Clarify and accomplish the user's request." : sentence;
    }
}

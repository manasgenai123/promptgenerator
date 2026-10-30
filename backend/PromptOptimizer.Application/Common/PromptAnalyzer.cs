using System.Text.RegularExpressions;

namespace PromptOptimizer.Application.Common;

/// <summary>
/// Lightweight, dependency-free helpers for estimating token counts and scoring
/// prompt quality. These are heuristics intended for UX feedback, not billing.
/// </summary>
public static class PromptAnalyzer
{
    /// <summary>
    /// Rough token estimate (~4 characters per token, with a word-count floor),
    /// matching the common OpenAI rule of thumb.
    /// </summary>
    public static int EstimateTokens(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0;
        }

        var charBased = (int)Math.Ceiling(text.Length / 4.0);
        var wordBased = (int)Math.Ceiling(CountWords(text) * 1.33);
        return Math.Max(charBased, wordBased);
    }

    /// <summary>
    /// Produces a 0-100 quality score based on structure, clarity signals and length.
    /// </summary>
    public static int ScoreQuality(string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            return 0;
        }

        double score = 40; // baseline

        var words = CountWords(prompt);

        // Reward a reasonable, non-bloated length.
        if (words is >= 20 and <= 400)
        {
            score += 15;
        }
        else if (words is >= 8 and < 20)
        {
            score += 8;
        }

        // Structure signals: headings, bullet points, numbered lists.
        if (Regex.IsMatch(prompt, @"(^|\n)\s*([-*]|\d+\.)\s", RegexOptions.Multiline))
        {
            score += 15;
        }

        // Explicit sections / directives.
        if (Regex.IsMatch(prompt, @"\b(objective|goal|context|requirements?|constraints?|output|format|example|acceptance criteria)\b",
                RegexOptions.IgnoreCase))
        {
            score += 20;
        }

        // Output format hints.
        if (Regex.IsMatch(prompt, @"\b(json|markdown|table|list|format|step[- ]by[- ]step)\b", RegexOptions.IgnoreCase))
        {
            score += 10;
        }

        return (int)Math.Clamp(Math.Round(score), 0, 100);
    }

    private static int CountWords(string text) =>
        Regex.Matches(text, @"\S+").Count;
}

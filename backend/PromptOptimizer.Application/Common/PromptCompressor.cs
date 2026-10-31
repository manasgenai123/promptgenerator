using System.Text.RegularExpressions;

namespace PromptOptimizer.Application.Common;

/// <summary>
/// Reduces a prompt's token footprint while preserving intent by removing
/// filler and politeness, tightening verbose phrasing and dropping duplicate
/// sentences. Used to guarantee an optimized prompt is never larger than the
/// original.
/// </summary>
public static class PromptCompressor
{
    // Wordy phrases rewritten to concise equivalents.
    private static readonly (string Pattern, string Replacement)[] VerbosePhrases =
    {
        (@"\bin order to\b", "to"),
        (@"\bdue to the fact that\b", "because"),
        (@"\bat this point in time\b", "now"),
        (@"\bin the event that\b", "if"),
        (@"\bfor the purpose of\b", "for"),
        (@"\bwith regard to\b", "about"),
        (@"\bwith regards to\b", "about"),
        (@"\bin terms of\b", "in"),
        (@"\ba large number of\b", "many"),
        (@"\bthe majority of\b", "most"),
        (@"\ba majority of\b", "most"),
        (@"\bthe fact that\b", "that"),
        (@"\bit is important to note that\b", ""),
        (@"\bplease note that\b", ""),
        (@"\bas a matter of fact\b", ""),
        (@"\bneeds? to be able to\b", "must"),
        (@"\bin a timely manner\b", "promptly"),
    };

    // Low-value filler and politeness words that add tokens without meaning.
    private static readonly string[] FillerPhrases =
    {
        @"\bi want you to\b",
        @"\bi would like you to\b",
        @"\bi need you to\b",
        @"\bi want\b",
        @"\bcould you please\b",
        @"\bcan you please\b",
        @"\bcould you\b",
        @"\bcan you\b",
        @"\bplease\b",
        @"\bkindly\b",
        @"\bas an ai\b",
        @"\bbasically\b",
        @"\bactually\b",
        @"\bsimply\b",
        @"\bjust\b",
        @"\bvery\b",
        @"\breally\b",
        @"\bquite\b",
        @"\bthat is to say\b",
    };

    /// <summary>Compresses the text, reducing token usage where possible.</summary>
    public static string Compress(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var result = text.Trim();

        foreach (var (pattern, replacement) in VerbosePhrases)
        {
            result = Regex.Replace(result, pattern, replacement, RegexOptions.IgnoreCase);
        }

        // Two passes: removing one filler can expose a multi-word filler that was
        // previously split by adjacent filler words.
        for (var pass = 0; pass < 2; pass++)
        {
            foreach (var filler in FillerPhrases)
            {
                result = Regex.Replace(result, filler, " ", RegexOptions.IgnoreCase);
            }

            result = Regex.Replace(result, @"[ \t]{2,}", " ");
        }

        result = RemoveDuplicateSentences(result);

        // Tidy whitespace and punctuation left by removals.
        result = Regex.Replace(result, @"[ \t]{2,}", " ");
        result = Regex.Replace(result, @"\s+([.,;:!?])", "$1");
        result = Regex.Replace(result, @"\n{3,}", "\n\n");
        result = Regex.Replace(result, @"(^|\n)[ \t]+", "$1");
        result = result.Trim();

        if (result.Length > 0)
        {
            result = char.ToUpperInvariant(result[0]) + result[1..];
        }

        return result;
    }

    /// <summary>
    /// Returns a prompt guaranteed to use no more tokens than <paramref name="original"/>.
    /// Prefers <paramref name="candidate"/>, then a compressed form, falling back
    /// to the compressed original.
    /// </summary>
    public static string EnsureNotLarger(string original, string candidate)
    {
        var originalTokens = PromptAnalyzer.EstimateTokens(original);

        if (!string.IsNullOrWhiteSpace(candidate) &&
            PromptAnalyzer.EstimateTokens(candidate) <= originalTokens)
        {
            return candidate;
        }

        var compressedCandidate = Compress(candidate);
        if (!string.IsNullOrWhiteSpace(compressedCandidate) &&
            PromptAnalyzer.EstimateTokens(compressedCandidate) <= originalTokens)
        {
            return compressedCandidate;
        }

        var compressedOriginal = Compress(original);
        return string.IsNullOrWhiteSpace(compressedOriginal) ? original.Trim() : compressedOriginal;
    }

    private static string RemoveDuplicateSentences(string text)
    {
        var sentences = Regex.Split(text, @"(?<=[.!?])\s+");
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var kept = new List<string>();

        foreach (var sentence in sentences)
        {
            var normalized = sentence.Trim();
            if (normalized.Length == 0)
            {
                continue;
            }

            if (seen.Add(normalized))
            {
                kept.Add(normalized);
            }
        }

        return string.Join(' ', kept);
    }
}

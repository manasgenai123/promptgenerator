using PromptOptimizer.Application.Interfaces;
using PromptOptimizer.Domain.Enums;

namespace PromptOptimizer.Application.Services;

/// <summary>
/// Provides the internal system prompt (base + per-style guidance).
/// </summary>
public class SystemPromptProvider : ISystemPromptProvider
{
    private const string BasePrompt =
        """
        You are an expert Prompt Engineer specializing in token-efficient prompts.

        Your job is to rewrite prompts so they are shorter and clearer while
        preserving the original intent and producing equal or better AI results.

        Hard rules:
        - The optimized prompt MUST use FEWER tokens than the original. This is the top priority.
        - Preserve the original intent and all essential requirements.
        - Remove filler, politeness, hedging and redundant words.
        - Replace verbose phrasing with concise equivalents.
        - Do NOT add boilerplate sections, headings or explanations that inflate length.
        - Only add structure if it does not increase the token count.
        - Improve grammar and clarity.

        Respond ONLY with a valid minified JSON object in exactly this shape and nothing else:
        {"optimizedPrompt":"<the shorter, improved prompt>","improvements":["<short point>","<short point>"]}
        Do not wrap the JSON in markdown fences.
        """;

    public string GetSystemPrompt(OptimizationType optimizationType)
    {
        var styleGuidance = optimizationType switch
        {
            OptimizationType.Technical =>
                "Optimize for a technical audience. Emphasize precise terminology, constraints, " +
                "assumptions, inputs/outputs, edge cases and non-functional requirements.",
            OptimizationType.Business =>
                "Optimize for a business audience. Emphasize goals, stakeholders, success metrics, " +
                "scope, deliverables and a professional tone. Avoid deep technical jargon.",
            OptimizationType.CodeGeneration =>
                "Optimize for code generation. Specify language/framework, function signatures, " +
                "inputs/outputs, error handling, performance constraints and that only code should be returned " +
                "when appropriate.",
            OptimizationType.UserStory =>
                "Optimize into an agile user story. Use the 'As a <role>, I want <goal>, so that <benefit>' " +
                "format and include clear, testable acceptance criteria.",
            OptimizationType.TestCase =>
                "Optimize into structured test cases. Include preconditions, test steps, test data and " +
                "expected results, covering positive, negative and edge scenarios.",
            _ =>
                "Apply general best-practice prompt optimization for a broad audience."
        };

        return $"{BasePrompt}\n\nStyle guidance for this request:\n- {styleGuidance}";
    }
}

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
        You are an expert Prompt Engineer.

        Your job is to transform vague, lengthy, or unstructured prompts into concise,
        high-quality prompts that produce better AI results.

        Rules:
        - Preserve the original intent.
        - Remove redundancy.
        - Improve grammar and readability.
        - Add structure and formatting.
        - Include clear objectives.
        - Include expected output format when appropriate.
        - Make the prompt suitable for AI models.
        - Return both the optimized prompt and a short explanation of improvements made.

        Respond ONLY with a valid minified JSON object in exactly this shape and nothing else:
        {"optimizedPrompt":"<the improved prompt>","improvements":["<short point>","<short point>"]}
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

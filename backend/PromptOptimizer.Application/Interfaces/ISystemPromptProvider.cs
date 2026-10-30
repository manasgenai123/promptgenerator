using PromptOptimizer.Domain.Enums;

namespace PromptOptimizer.Application.Interfaces;

/// <summary>
/// Builds the internal system prompt used to instruct the LLM, tailored per
/// optimization style.
/// </summary>
public interface ISystemPromptProvider
{
    string GetSystemPrompt(OptimizationType optimizationType);
}

using FluentValidation;
using PromptOptimizer.Application.DTOs;

namespace PromptOptimizer.Application.Validators;

/// <summary>
/// Validates incoming optimization requests.
/// </summary>
public class OptimizePromptRequestValidator : AbstractValidator<OptimizePromptRequest>
{
    public const int MaxPromptLength = 20_000;

    public OptimizePromptRequestValidator()
    {
        RuleFor(x => x.Prompt)
            .NotEmpty().WithMessage("Prompt is required.")
            .MinimumLength(5).WithMessage("Prompt must be at least 5 characters.")
            .MaximumLength(MaxPromptLength)
            .WithMessage($"Prompt must not exceed {MaxPromptLength} characters.");

        RuleFor(x => x.OptimizationType)
            .IsInEnum().WithMessage("Invalid optimization type.");
    }
}

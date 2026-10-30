using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PromptOptimizer.Application.Interfaces;
using PromptOptimizer.Application.Services;
using PromptOptimizer.Application.Validators;

namespace PromptOptimizer.Application;

/// <summary>
/// Dependency injection registrations for the Application layer.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPromptOptimizationService, PromptOptimizationService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddSingleton<ISystemPromptProvider, SystemPromptProvider>();

        services.AddValidatorsFromAssemblyContaining<OptimizePromptRequestValidator>();

        return services;
    }
}

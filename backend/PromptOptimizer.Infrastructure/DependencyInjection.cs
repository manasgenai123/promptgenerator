using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PromptOptimizer.Application.Interfaces;
using PromptOptimizer.Infrastructure.OpenAI;
using PromptOptimizer.Infrastructure.Persistence;

namespace PromptOptimizer.Infrastructure;

/// <summary>
/// Dependency injection registrations for the Infrastructure layer
/// (LLM provider + persistence).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<OpenAIOptions>(configuration.GetSection(OpenAIOptions.SectionName));

        var openAiOptions = configuration.GetSection(OpenAIOptions.SectionName).Get<OpenAIOptions>()
                            ?? new OpenAIOptions();

        services.AddHttpClient<IOpenAIService, OpenAIService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(openAiOptions.TimeoutSeconds);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        });

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));
        }
        else
        {
            // No SQL Server configured: fall back to the EF in-memory provider
            // so the app runs out of the box (history persists for the process).
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("PromptOptimizer"));
        }

        services.AddScoped<IPromptRepository, PromptRepository>();

        return services;
    }
}

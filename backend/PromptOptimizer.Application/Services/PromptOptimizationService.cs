using Microsoft.Extensions.Logging;
using PromptOptimizer.Application.Common;
using PromptOptimizer.Application.DTOs;
using PromptOptimizer.Application.Interfaces;
using PromptOptimizer.Domain.Entities;

namespace PromptOptimizer.Application.Services;

/// <summary>
/// Default implementation of <see cref="IPromptOptimizationService"/>.
/// </summary>
public class PromptOptimizationService : IPromptOptimizationService
{
    private readonly IOpenAIService _openAIService;
    private readonly ISystemPromptProvider _systemPromptProvider;
    private readonly IPromptRepository _repository;
    private readonly ILogger<PromptOptimizationService> _logger;

    public PromptOptimizationService(
        IOpenAIService openAIService,
        ISystemPromptProvider systemPromptProvider,
        IPromptRepository repository,
        ILogger<PromptOptimizationService> logger)
    {
        _openAIService = openAIService;
        _systemPromptProvider = systemPromptProvider;
        _repository = repository;
        _logger = logger;
    }

    public async Task<OptimizePromptResponse> OptimizeAsync(
        OptimizePromptRequest request,
        string? userId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Optimizing prompt of {Length} chars with style {Style}",
            request.Prompt.Length, request.OptimizationType);

        var systemPrompt = _systemPromptProvider.GetSystemPrompt(request.OptimizationType);

        var result = await _openAIService.GetOptimizedPromptAsync(
            systemPrompt, request.Prompt, cancellationToken);

        // Guarantee the optimized prompt never uses more tokens than the original.
        var optimizedPrompt = PromptCompressor.EnsureNotLarger(request.Prompt, result.OptimizedPrompt);

        var response = new OptimizePromptResponse
        {
            OriginalPrompt = request.Prompt,
            OptimizedPrompt = optimizedPrompt,
            Improvements = result.Improvements,
            OptimizationType = request.OptimizationType.ToString(),
            QualityScore = PromptAnalyzer.ScoreQuality(optimizedPrompt),
            OriginalTokenEstimate = PromptAnalyzer.EstimateTokens(request.Prompt),
            OptimizedTokenEstimate = PromptAnalyzer.EstimateTokens(optimizedPrompt)
        };

        ApplyTokenUsage(response, result, systemPrompt, request.Prompt, optimizedPrompt);

        await PersistAsync(request, response, userId, cancellationToken);

        return response;
    }

    private static void ApplyTokenUsage(
        OptimizePromptResponse response,
        PromptOptimizationResult result,
        string systemPrompt,
        string userPrompt,
        string optimizedPrompt)
    {
        if (result.Usage is { } usage)
        {
            response.PromptTokens = usage.PromptTokens;
            response.CompletionTokens = usage.CompletionTokens;
            response.TotalTokens = usage.TotalTokens;
            response.TokenUsageSource = "Api";
            return;
        }

        // Offline / no provider usage: approximate what the call would have consumed.
        response.PromptTokens = PromptAnalyzer.EstimateTokens(systemPrompt)
                                + PromptAnalyzer.EstimateTokens(userPrompt);
        response.CompletionTokens = PromptAnalyzer.EstimateTokens(optimizedPrompt);
        response.TotalTokens = response.PromptTokens + response.CompletionTokens;
        response.TokenUsageSource = "Estimated";
    }

    private async Task PersistAsync(
        OptimizePromptRequest request,
        OptimizePromptResponse response,
        string? userId,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = new PromptOptimization
            {
                OriginalPrompt = request.Prompt,
                OptimizedPrompt = response.OptimizedPrompt,
                OptimizationType = request.OptimizationType,
                Improvements = string.Join('\n', response.Improvements),
                QualityScore = response.QualityScore,
                OriginalTokenEstimate = response.OriginalTokenEstimate,
                OptimizedTokenEstimate = response.OptimizedTokenEstimate,
                UserId = userId
            };

            await _repository.AddAsync(entity, cancellationToken);
        }
        catch (Exception ex)
        {
            // Persistence is best-effort; never fail the request because history could not be saved.
            _logger.LogWarning(ex, "Failed to persist prompt optimization history");
        }
    }
}

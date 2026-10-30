using PromptOptimizer.Application.DTOs;
using PromptOptimizer.Application.Interfaces;

namespace PromptOptimizer.Application.Services;

/// <summary>
/// Default implementation of <see cref="IDashboardService"/>.
/// </summary>
public class DashboardService : IDashboardService
{
    private const int RecentCount = 10;

    private readonly IPromptRepository _repository;

    public DashboardService(IPromptRepository repository)
    {
        _repository = repository;
    }

    public async Task<DashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var all = await _repository.GetAllAsync(cancellationToken);

        if (all.Count == 0)
        {
            return new DashboardStatsDto();
        }

        var tokensSaved = all.Sum(x => Math.Max(0, x.OriginalTokenEstimate - x.OptimizedTokenEstimate));

        return new DashboardStatsDto
        {
            TotalOptimizations = all.Count,
            AverageQualityScore = Math.Round(all.Average(x => x.QualityScore), 1),
            TotalTokensSaved = tokensSaved,
            OptimizationsByType = all
                .GroupBy(x => x.OptimizationType.ToString())
                .ToDictionary(g => g.Key, g => g.Count()),
            Recent = all
                .OrderByDescending(x => x.CreatedAtUtc)
                .Take(RecentCount)
                .Select(x => new RecentOptimizationDto
                {
                    Id = x.Id,
                    OptimizationType = x.OptimizationType.ToString(),
                    QualityScore = x.QualityScore,
                    CreatedAtUtc = x.CreatedAtUtc
                })
                .ToList()
        };
    }
}

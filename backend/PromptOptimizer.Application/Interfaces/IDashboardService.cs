using PromptOptimizer.Application.DTOs;

namespace PromptOptimizer.Application.Interfaces;

/// <summary>
/// Produces aggregated statistics about prompt optimizations for the dashboard.
/// </summary>
public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken = default);
}

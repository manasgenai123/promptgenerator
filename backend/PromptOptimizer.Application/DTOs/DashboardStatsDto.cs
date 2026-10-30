namespace PromptOptimizer.Application.DTOs;

/// <summary>Aggregated statistics for the dashboard.</summary>
public class DashboardStatsDto
{
    public int TotalOptimizations { get; set; }

    public double AverageQualityScore { get; set; }

    public int TotalTokensSaved { get; set; }

    public Dictionary<string, int> OptimizationsByType { get; set; } = new();

    public IReadOnlyList<RecentOptimizationDto> Recent { get; set; } = new List<RecentOptimizationDto>();
}

public class RecentOptimizationDto
{
    public Guid Id { get; set; }

    public string OptimizationType { get; set; } = string.Empty;

    public int QualityScore { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
}

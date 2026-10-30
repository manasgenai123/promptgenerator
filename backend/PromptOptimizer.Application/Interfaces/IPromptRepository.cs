using PromptOptimizer.Domain.Entities;

namespace PromptOptimizer.Application.Interfaces;

/// <summary>
/// Persistence abstraction for optimized prompts.
/// </summary>
public interface IPromptRepository
{
    Task AddAsync(PromptOptimization entity, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PromptOptimization>> GetRecentAsync(int take, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PromptOptimization>> GetAllAsync(CancellationToken cancellationToken = default);
}

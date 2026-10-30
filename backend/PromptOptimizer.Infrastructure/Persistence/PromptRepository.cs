using Microsoft.EntityFrameworkCore;
using PromptOptimizer.Application.Interfaces;
using PromptOptimizer.Domain.Entities;

namespace PromptOptimizer.Infrastructure.Persistence;

public class PromptRepository : IPromptRepository
{
    private readonly AppDbContext _dbContext;

    public PromptRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(PromptOptimization entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.PromptOptimizations.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PromptOptimization>> GetRecentAsync(int take, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PromptOptimizations
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PromptOptimization>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.PromptOptimizations
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
}

using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class AlgorithmService(AppDbContext context) {
    public async Task<Algorithm?> SetStandardAlgorithm(int id) {
        await using var transaction = await context.Database.BeginTransactionAsync();
        var algorithm = await context.Algorithms.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        if (algorithm == null) return null;
        // Serialize selections within a case so concurrent requests cannot select two standards.
        await context.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT 1 FROM \"AlgorithmCases\" WHERE \"Id\" = {algorithm.AlgorithmCaseId} FOR UPDATE");
        await context.Algorithms.Where(a => a.AlgorithmCaseId == algorithm.AlgorithmCaseId && a.IsStandard)
            .ExecuteUpdateAsync(update => update.SetProperty(a => a.IsStandard, false));
        var updated = await context.Algorithms.Where(a => a.Id == id)
            .ExecuteUpdateAsync(update => update.SetProperty(a => a.IsStandard, true));
        if (updated == 0) return null;
        await transaction.CommitAsync();
        algorithm.IsStandard = true;
        return algorithm;
    }

    /**
     * Get a specific algorithm set by name
     */
    public async Task<AlgorithmSet?> GetAlgorithmSet(string name) {
        return await context.AlgorithmSets
            .Include(s => s.Cases)
            .ThenInclude(c => c.Algorithms)
            .Include(s => s.Groups)
            .ThenInclude(g => g.Cases)
            .ThenInclude(c => c.Algorithms)
            .FirstOrDefaultAsync(s => s.Name == name);
    }

    /**
     * Get all algorithm sets (OLL, PLL, etc.)
     */
    public async Task<List<AlgorithmSet>> GetAlgorithmSets() {
        return await context.AlgorithmSets
            .Include(s => s.Cases)
            .ThenInclude(c => c.Algorithms)
            .Include(s => s.Groups)
            .ThenInclude(g => g.Cases)
            .ThenInclude(c => c.Algorithms)
            .ToListAsync();
    }

    /**
     * Get a specific algorithm case by ID
     */
    public async Task<AlgorithmCase?> GetAlgorithmCase(int id) {
        return await context.AlgorithmCases
            .Include(c => c.Algorithms)
            .Include(c => c.AlgorithmGroup)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    /**
     * Get a specific algorithm case by name
     */
    public async Task<AlgorithmCase?> GetAlgorithmCase(string name) {
        return await context.AlgorithmCases
            .Include(c => c.Algorithms)
            .Include(c => c.AlgorithmGroup)
            .FirstOrDefaultAsync(c => c.Name == name);
    }
}

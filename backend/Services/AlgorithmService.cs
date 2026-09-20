using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class AlgorithmService(AppDbContext context) {
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

using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public static class AlgorithmSeeder {
    public static async Task SeedAsync(AppDbContext context) {
        if (await context.AlgorithmSets.AnyAsync(s => s.Name == "PLL")) {
            return;
        }

        var pll = new AlgorithmSet {
            Name = "PLL"
        };

        var pllCases = new[] {
            "Aa",
            "Ab",
            "F",
            "Ga",
            "Gb",
            "Gc",
            "Gd",
            "Ja",
            "Jb",
            "Ra",
            "Rb",
            "T",
            "E",
            "Na",
            "Nb",
            "V",
            "Y",
            "H",
            "Ua",
            "Ub",
            "Z",
        };

        foreach (var c in pllCases) {
            pll.Cases.Add(new AlgorithmCase {
                Name = c,
            });
        }

        context.AlgorithmSets.Add(pll);

        await context.SaveChangesAsync();
    }
}

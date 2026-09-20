using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public static class AlgorithmSeeder {
    public static async Task SeedAsync(AppDbContext context) {
        await SeedPllCases(context);
        await SeedOllCases(context);
    }

    private static async Task SeedPllCases(AppDbContext context) {
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

    private static async Task SeedOllCases(AppDbContext context) {
        if (await context.AlgorithmSets.AnyAsync(s => s.Name == "OLL")) {
            return;
        }

        var oll = new AlgorithmSet {
            Name = "OLL"
        };

        var ollCases = new[] {
            "All Corners Orientated",
            "Awkward Shapes",
            "C Shapes",
            "Dot Case",
            "Fish Shapes",
            "Knight Move Shapes",
            "L shapes",
            "Lightning Shapes",
            "Line Shapes",
            "OCLL",
            "P Shapes",
            "Square Shapes",
            "T Shapes",
            "W Shapes",
        };

        foreach (var c in ollCases) {
            oll.Cases.Add(new AlgorithmCase {
                Name = c,
            });
        }

        context.AlgorithmSets.Add(oll);

        await context.SaveChangesAsync();
    }
}

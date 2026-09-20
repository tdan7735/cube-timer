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
        var oll = await context.AlgorithmSets
            .Include(s => s.Groups)
            .FirstOrDefaultAsync(s => s.Name == "OLL");

        if (oll?.Groups.Any() == true) {
            return;
        }

        if (oll == null) {
            oll = new AlgorithmSet {
                Name = "OLL"
            };
            context.AlgorithmSets.Add(oll);
        }

        var ollGroups = new Dictionary<string, int[]> {
            ["All Corners Oriented"] = [28, 57],
            ["Awkward Shapes"] = [29, 30, 41, 42],
            ["C Shapes"] = [34, 46],
            ["Dot Case"] = [1, 2, 3, 4, 17, 18, 19, 20],
            ["Fish Shapes"] = [9, 10, 35, 37],
            ["Knight Move Shapes"] = [13, 14, 15, 16],
            ["L Shapes"] = [47, 48, 49, 50, 53, 54],
            ["Lightning Shapes"] = [7, 8, 11, 12, 39, 40],
            ["Line Shapes"] = [51, 52, 55, 56],
            ["OCLL"] = [21, 22, 23, 24, 25, 26, 27],
            ["P Shapes"] = [31, 32, 43, 44],
            ["Square Shapes"] = [5, 6],
            ["T Shapes"] = [33, 45],
            ["W Shapes"] = [36, 38],
        };

        foreach (var (groupName, caseNumbers) in ollGroups) {
            var group = new AlgorithmGroup {
                Name = groupName,
            };

            foreach (var caseNumber in caseNumbers) {
                group.Cases.Add(new AlgorithmCase {
                    Name = $"OLL {caseNumber}",
                    CaseNumber = caseNumber,
                    AlgorithmSet = oll,
                });
            }

            oll.Groups.Add(group);
        }

        await context.SaveChangesAsync();
    }
}

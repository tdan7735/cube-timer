using backend.Models;

namespace backend.Services;

/*
 * -1 implies DNF
*/
public class StatisticsService {

    /**
     * Calculates the average of all solve times.
     * Solves with a DNF penalty are ignored.
    */
    public double? CalculateTotalAverage(List<Solve> solves) {
        if (solves.Count == 0) {
            return null;
        }

        double sum = 0;
        int numDnf = 0;
        foreach (var solve in solves) {
            if (solve.Penalty != Penalty.DNF) {
                sum += solve.FinalTime();
            }
            else {
                numDnf++;
            }
        }

        return sum / (solves.Count - numDnf);
    }

    /**
     * Calculates the average of the solve times for the most recent 5 solves.
     * The fastest and slowest solves are ignored.
     * If there are more than 2 solves with a DNF penalty, the ao5 is DNF.
    */
    public double? CalculateAo5(List<Solve> solves) {
        var recentSolves = solves
            .OrderByDescending(s => s.TimeSolved)
            .Take(5)
            .ToList();

        if (recentSolves.Count < 5) {
            return null;
        }

        return AoHelper(recentSolves, 5);
    }

    /**
     * Calculates the average of the solve times for the most recent 12 solves.
     * The fastest and slowest solves are ignored.
     * If there are more than 2 solves with a DNF penalty, the ao12 is DNF.
    */
    public double? CalculateAo12(List<Solve> solves) {
        var recentSolves = solves
           .OrderByDescending(s => s.TimeSolved)
           .Take(12)
           .ToList();

        if (recentSolves.Count < 12) {
            return null;
        }

        return AoHelper(recentSolves, 12);
    }

    /**
     * Calculates the average of the solve times for the most recent 50 solves.
     * The fastest and slowest 3 solves are ignored
     * If there are more than 3 solves with a DNF penalty, the ao50 is DNF.
    */
    public double? CalculateAo50(List<Solve> solves) {
        var recentSolves = solves
            .OrderByDescending(s => s.TimeSolved)
            .Take(50)
            .ToList();

        if (recentSolves.Count < 50) {
            return null;
        }

        return AoHelper(recentSolves, 50);
    }

    /**
     * Calculates the average of the solve times for the most recent 100 solves.
     * The fastest and slowest 5 solves are ignored
     * If there are more than 5 solves with a DNF penalty, the ao100 is DNF.
    */
    public double? CalculateAo100(List<Solve> solves) {
        var recentSolves = solves.OrderByDescending(s => s.TimeSolved)
            .Take(100)
            .ToList();

        if (recentSolves.Count < 100) {
            return null;
        }

        return AoHelper(recentSolves, 100);
    }

    /**
     * Returns the fastest solve time
    */
    public double? GetPersonalBest(List<Solve> solves) {
        var sortedSolves = SortSolves(solves);

        if (sortedSolves.Count == 0) {
            return null;
        }

        return sortedSolves[0].FinalTime();
    }

    /**
     * Helper function for Ao calculations
     * Calculates the average of the solve times for the most recent num solves
     * 5% of the slowest and fastest times are removed
     * If 5% isn't a whole number, it is rounded up
     * After removing 5% of the slowest and fastest times, the remaining times are averaged
     * If any of the remaining times are DNF, the ao is DNF
    */
    static private double AoHelper(List<Solve> solves, int num) {
        double sum = 0;
        int numDnf = 0;

        var sortedSolves = SortSolves(solves);
        int leftBound = (int)Math.Ceiling(0.05 * num);
        int rightBound = (int)Math.Ceiling(0.05 * num);

        for (int i = leftBound; i < num - rightBound; i++) {
            var solve = sortedSolves[i];
            if (solve.Penalty != Penalty.DNF) {
                sum += solve.FinalTime();
            }
            else {
                numDnf++;
            }
        }

        if (numDnf >= 1) {
            return -1;
        }

        return sum / (num - leftBound - rightBound);
    }

    /**
     * Helper function for sorting the solves from fastest to slowest
     * If a penalty is DNF, it will be considered as the last solve
    */
    static private List<Solve> SortSolves(List<Solve> solves) {
        var sortedSolves = solves
            .OrderBy(s => s.Penalty == Penalty.DNF)
            .ThenBy(s => s.FinalTime())
            .ToList();

        return sortedSolves;
    }
}

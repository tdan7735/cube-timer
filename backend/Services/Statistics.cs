using backend.Models;

namespace backend.Services;

public class Statistics {

    /**
     * Calculates the average of all solve times.
     * Solves with a DNF penalty are ignored.
    */
    public double CalculateTotalAverage(List<Solve> solves) {
        double sum = 0;
        foreach (var solve in solves) {
            if (solve.Penalty != Penalty.DNF) {
                sum += solve.FinalTime();
            }
        }
        return sum / solves.Count;
    }

    /**
     * Calculates the average of the solve times for the most recent 5 solves.
     * The fastest and slowest solves are ignored.
     * If there are more than 2 solves with a DNF penalty, the ao5 is DNF.
    */
    public double CalculateAo5(List<Solve> solves) {
        var recentSolves = solves.OrderByDescending(s => s.TimeSolved).Take(5).ToList();

        if (recentSolves == null || recentSolves.Count == 0) {
            return -1;
        }

        if (recentSolves.Count < 5) {
            return -1;
        }

        return AoHelper(recentSolves, 5);
    }

    /**
     * Calculates the average of the solve times for the most recent 12 solves.
     * The fastest and slowest solves are ignored.
     * If there are more than 2 solves with a DNF penalty, the ao12 is DNF.
    */
    public double CalculateAo12(List<Solve> solves) {
        var recentSolves = solves.OrderByDescending(s => s.TimeSolved).Take(12).ToList();

        if (recentSolves == null || recentSolves.Count == 0) {
            return -1;
        }

        if (recentSolves.Count < 12) {
            return -1;
        }

        return AoHelper(recentSolves, 12);
    }

    /**
     * helper function for Ao calculations
     * Calculates the average of the solve times for the most recent num solves.
     * The fastest and slowest solves are ignored.
     * If there are more than 2 solves with a DNF penalty, then return -1
    */
    static private double AoHelper(List<Solve> solves, int num) {
        double sum = 0;
        int numDnf = 0;
        double max = 0;
        double min = 0;

        foreach (var solve in solves) {
            if (solve.Penalty != Penalty.DNF) {
                sum += solve.FinalTime();
            }
            else {
                numDnf++;
            }

            if (solve.FinalTime() < min) {
                min = solve.FinalTime();
            }

            if (solve.FinalTime() > max) {
                max = solve.FinalTime();
            }
        }

        if (numDnf >= num) {
            return -1;
        }

        sum = sum - min - max;
        return sum / (num - 2);
    }
}

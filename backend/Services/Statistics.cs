using backend.Models;

namespace backend.Services;

public class Statistics {

    /**
     * Calculates the average of all solve times.
     * Solves with a DNF penalty are ignored.
    */
    public double CalculateAverage(List<Solve> solves) {
        double sum = 0;
        foreach (var solve in solves) {
            if (solve.Penalty != Penalty.DNF) {
                sum += solve.FinalTime();
            }
        }
        return sum / solves.Count;
    }

    /**
     * Calculates the average of the solve times for the most recent 3 solves.
     * If there is a DNF penalty, the ao3 is DNF.
    */
    public double CalculateAo3(List<Solve> solves) {
        var recentSolves = solves.OrderByDescending(s => s.TimeSolved).Take(3).ToList();

        if (recentSolves == null || recentSolves.Count == 0) {
            return -1;
        }

        if (recentSolves.Count < 3) {
            return -1;
        }

        double sum = 0;
        int numDnf = 0;
        foreach (var solve in recentSolves) {
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

        return sum / 3;
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

        double sum = 0;
        int numDnf = 0;
        double min = recentSolves[0].FinalTime();
        double max = 0;
        foreach (var solve in recentSolves) {
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

        if (numDnf >= 2) {
            return -1;
        }

        sum = sum - min - max;
        return sum / 3;
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

        double sum = 0;
        int numDnf = 0;
        double min = recentSolves[0].FinalTime();
        double max = 0;
        foreach (var solve in recentSolves) {
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

        if (numDnf > 2) {
            return -1;
        }

        sum = sum - min - max;
        return sum / 10;
    }
}

namespace backend.Models;

public enum AlgorithmLearningStatus {
    NotLearned,
    Learning,
    Learned,
}

public enum TrainingFocus {
    All,
    Slowest,
}

public enum TrainingOrder {
    Balanced,
    Random,
}

public class UserAlgorithmCaseProgress {
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int AlgorithmCaseId { get; set; }
    public AlgorithmCase AlgorithmCase { get; set; } = null!;
    public AlgorithmLearningStatus Status { get; set; } = AlgorithmLearningStatus.NotLearned;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class TrainingPreferences {
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int AlgorithmSetId { get; set; }
    public AlgorithmSet AlgorithmSet { get; set; } = null!;
    public bool IncludeNotLearned { get; set; } = true;
    public bool IncludeLearning { get; set; } = true;
    public bool IncludeLearned { get; set; } = true;
    public TrainingFocus Focus { get; set; } = TrainingFocus.All;
    public int SlowestCount { get; set; } = 10;
    public TrainingOrder Order { get; set; } = TrainingOrder.Balanced;
    // Null means all currently available cases, so newly added cases are included.
    public int[]? SelectedCaseIds { get; set; }
}

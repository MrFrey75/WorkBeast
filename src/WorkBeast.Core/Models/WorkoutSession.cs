namespace WorkBeast.Core.Models;

public class WorkoutSession : BaseEntity
{
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string Notes { get; set; } = string.Empty;
    public List<LoggedExercise> LoggedExercises { get; set; } = new();
}

public class LoggedExercise : BaseEntity
{
    public int WorkoutSessionOid { get; set; }
    public int ExerciseOid { get; set; }
    public Exercise Exercise { get; set; } = null!;

    // Used to group exercises together for supersets/rounds
    public int Sequence { get; set; } 
    public int? ParentGroupId { get; set; } 

    public List<WorkoutSet> Sets { get; set; } = new();
}

public class WorkoutSet : BaseEntity
{
    public int LoggedExerciseOid { get; set; }
    
    public int Weight { get; set; } // 0 for bodyweight
    public int Reps { get; set; }
    public int? ResistanceLevel { get; set; } // For machines/bands
    
    public string SetType { get; set; } = "Normal"; // Normal, Drop Set, Warmup
    public int SortOrder { get; set; }
}
namespace FitnessTracker.Models;

public class DayProgressModel
{
    public string DayLabel { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}

public class ExerciseItemModel
{
    public string Name { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
}

public class WeekDayWorkoutModel
{
    public DateTime Date { get; set; }
    public string DayLabel { get; set; } = string.Empty;
    public Workout? Workout { get; set; }
    public bool IsCompleted { get; set; }
}

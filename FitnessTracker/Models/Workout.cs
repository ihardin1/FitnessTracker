using System.Collections.ObjectModel;

namespace FitnessTracker.Models;

public class Workout
{
    public string WorkoutName { get; set; } = string.Empty;

    // Date for which this workout applies
    public DateTime Date { get; set; } = DateTime.Today;

    public ObservableCollection<Exercise> Exercises { get; set; } = new();

    // Whether the workout has been completed
    public bool IsCompleted { get; set; }
}
namespace FitnessTracker.Models;

public class Exercise
{
    public string Name { get; set; } = string.Empty;

    public double Weight { get; set; }

    public int Reps { get; set; }

    public int Sets { get; set; }

    public DateTime Date { get; set; } = DateTime.Today;

    public bool IsCompleted { get; set; }
}
namespace FitnessTracker.Models;

public class Meal
{
    public string Name { get; set; } = string.Empty;

    public string MealType { get; set; } = string.Empty;

    public string Day { get; set; } = string.Empty;

    public int Calories { get; set; }
}
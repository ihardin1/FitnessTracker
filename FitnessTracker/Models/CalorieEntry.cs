namespace FitnessTracker.Models;

public class CalorieEntry
{
    public string FoodName { get; set; } = string.Empty;

    public string MealType { get; set; } = string.Empty;

    public int Calories { get; set; }

    public DateTime Date { get; set; } = DateTime.Today;
}
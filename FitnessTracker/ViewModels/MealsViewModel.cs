using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessTracker.Models;
using FitnessTracker.Services;

namespace FitnessTracker.ViewModels;

public partial class MealsViewModel : ObservableObject
{
    [ObservableProperty]
    private string mealName = string.Empty;

    [ObservableProperty]
    private string caloriesText = string.Empty;

    public ObservableCollection<Meal> Meals =>
        FitnessService.Meals;

    public MealsViewModel()
    {
        FitnessService.OnMealsChanged += RefreshMeals;
    }

    private void RefreshMeals()
    {
        OnPropertyChanged(nameof(Meals));
    }

    [RelayCommand]
    private async Task AddMeal()
    {
        if (string.IsNullOrWhiteSpace(MealName))
        {
            await Shell.Current.DisplayAlertAsync(
                "Missing Meal",
                "Please enter a meal name.",
                "OK");

            return;
        }

        if (!int.TryParse(CaloriesText, out int calories) ||
            calories <= 0)
        {
            await Shell.Current.DisplayAlertAsync(
                "Invalid Calories",
                "Please enter a valid calorie amount.",
                "OK");

            return;
        }

        Meal meal = new()
        {
            Name = MealName.Trim(),
            Calories = calories
        };

        FitnessService.AddMeal(meal);

        MealName = string.Empty;
        CaloriesText = string.Empty;

        RefreshMeals();

        await Shell.Current.DisplayAlertAsync(
            "Meal Added",
            $"{meal.Name} was saved.",
            "OK");
    }

    [RelayCommand]
    private async Task DeleteMeal(Meal? meal)
    {
        if (meal == null)
        {
            return;
        }

        bool shouldDelete =
            await Shell.Current.DisplayAlertAsync(
                "Delete Meal",
                $"Remove {meal.Name}?",
                "Delete",
                "Cancel");

        if (!shouldDelete)
        {
            return;
        }

        FitnessService.DeleteMeal(meal);

        RefreshMeals();
    }
}
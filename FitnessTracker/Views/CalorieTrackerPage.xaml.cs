using FitnessTracker.ViewModels;

namespace FitnessTracker.Views;

public partial class CalorieTrackerPage : ContentPage
{
    public CalorieTrackerPage()
    {
        InitializeComponent();

        BindingContext =
            new CalorieTrackerViewModel();
    }
}
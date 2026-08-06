using FitnessTracker.ViewModels;

namespace FitnessTracker.Views;

public partial class WorkoutPage : ContentPage
{
    public WorkoutPage()
    {
        InitializeComponent();

        BindingContext = new WorkoutViewModel();
    }
}
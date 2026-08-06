using FitnessTracker.ViewModels;

namespace FitnessTracker.Views;

public partial class ProgressPage : ContentPage
{
    public ProgressPage()
    {
        InitializeComponent();

        BindingContext = new ProgressViewModel();
    }
}
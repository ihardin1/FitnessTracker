using FitnessTracker.ViewModels;

namespace FitnessTracker.Views;

public partial class GoalsPage : ContentPage
{
	public GoalsPage()
	{
		InitializeComponent();

        BindingContext = new GoalsViewModel();
    }
}
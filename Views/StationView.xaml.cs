namespace VSmauiApp.Views;
using ViewModels;
using VSmauiApp.Controls;

public partial class StationView : ContentView
{
	public StationView()
	{
		InitializeComponent();
	}
}
public class StationsListView : Controls.MyListView
{
    protected override View CreateItemView()
    {
        return new StationView();
    }
}

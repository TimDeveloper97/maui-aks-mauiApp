namespace VSmauiApp.Views;

public partial class HistoryItemView : ContentView
{
	public HistoryItemView()
	{
		InitializeComponent();
	}
}


public class HistoryListView : Controls.MyListView
{
    protected override View CreateItemView()
    {
        return new Views.HistoryItemView();
    }
}

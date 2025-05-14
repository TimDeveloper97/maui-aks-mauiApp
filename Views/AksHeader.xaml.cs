namespace VSmauiApp.Views;

public partial class AksHeader : ContentView
{
	public AksHeader()
	{
		InitializeComponent();

		this.BindingContext = BaseViewModel.User;
	}
}
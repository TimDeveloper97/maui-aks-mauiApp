using VSmauiApp.ViewModels;

namespace VSmauiApp.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();

		btnSubmit.Clicked += (s, e) => {
			var vm = (LoginViewModel)BindingContext;
			if (vm.UserName == string.Empty || vm.Password == string.Empty)
			{
				App.DisplayAlert("Tên đăng nhập và mật khẩu không được để trống");
				return;
			}

			vm.Submit();
		};
	}
}
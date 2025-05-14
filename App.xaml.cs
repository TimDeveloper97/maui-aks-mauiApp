using System.Threading.Tasks;
using System.Xml.Serialization;
using VSmauiApp.Controls;

namespace VSmauiApp
{
    public partial class App : Application
    {
        static string? _currentUrl;
        static public Action<string>? OnCurrentUrlChanged; 
        static public async void Request(string url)
        {
            if (_currentUrl != url)
            {
                var task = Shell.Current.GoToAsync(url);
                await task;

                if (task.IsCompleted)
                {
                    _currentUrl = url;
                    OnCurrentUrlChanged?.Invoke(url);
                }
            }
        }
        static public async void DisplayAlert(string message, string cancel = "OK")
        {
            if (Current?.MainPage != null)
            {
                await Current.MainPage.DisplayAlert("A.K.S", message, cancel);
            }
        }

        static public void LoginSuccess() 
        {
            if (Current != null) Current.MainPage = new AppShell();
        }
        static public void Logout()
        {
            if (Current != null) Current.MainPage = new Pages.LoginPage();
        }
        public App()
        {

            InitializeComponent();

            SvgIcon.Register("device");
            SvgIcon.Register("setting");
            SvgIcon.Register("history");
            SvgIcon.Register("home");
            SvgIcon.Register("bar-chart");
            SvgIcon.Register("user");
            SvgIcon.Register("bolt");

            MainPage = new Pages.LoginPage();

            //MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            //Request("//home");
            //Request("//stations");
            base.OnStart();
        }
    }
}

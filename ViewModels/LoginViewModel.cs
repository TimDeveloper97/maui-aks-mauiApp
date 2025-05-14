using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSmauiApp.ViewModels
{
    class LoginViewModel : BaseViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public LoginViewModel() 
        {
            UserName = "admin";
            Password = "1234";
        }
        public void Submit() {
            App.LoginSuccess();
        }
    }
}

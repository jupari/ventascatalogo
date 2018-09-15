using GalaSoft.MvvmLight.Command;
using System;
using System.Windows.Input;
using ventascatalogo.Views;
using Xamarin.Forms;
using ventascatalogo.Helpers;

namespace ventascatalogo.ViewModels
{
   public class MenuItemViewModel
   {
        #region Properties
        public string Icon { get; set; }

        public string Title { get; set; }

        public string PageName { get; set; }
        #endregion
        #region Constructores

        #endregion
        #region Metodos

        #endregion
        #region Comandos
        public ICommand GotoCommand
        {
            get
            {
                return new RelayCommand(Goto);
            }
        }
        private void Goto()
        {
            if (this.PageName == "LoginPage")
            {
                Settings.AccessToken = string.Empty;
                Settings.TokenType = string.Empty;
                Settings.IsRemembered = true;
                MainViewModel.GetInstance().Login = new LoginViewModel();
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }
        #endregion

    }
}

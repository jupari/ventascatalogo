namespace ventascatalogo.ViewModels
{
    using System;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Xamarin.Forms;
    using Services;
    using ventascatalogo.Helpers;
    using ventascatalogo.Views;

    public class LoginViewModel:BaseViewModels
    {
        #region Atributos
        private ApiService apiService;
        private bool isRunning;
        private bool isEnabled;
        #endregion
        #region Propiedades
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsRemembered { get; set; }
        public bool IsRunning
        {
            get { return this.isRunning; }
            set { this.SetValue(ref this.isRunning, value);
            }
        }
        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set { this.SetValue(ref this.isEnabled, value); }
        }
        #endregion
        #region Constructores
        public LoginViewModel()
        {
            this.apiService = new ApiService();
            this.IsEnabled = true;
            this.IsRemembered = true;
        }
        #endregion
        #region Commandos
        public ICommand LoginCommand
        {
            get
            {
                return new RelayCommand(Login);
            }
            
        }

        private async void Login()
        {
            if (string.IsNullOrEmpty(this.Email))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Debe Ingresar Un Correo",
                    "Aceptar"
                    );
                return;
            }
            if (string.IsNullOrEmpty(this.Password))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Debe Ingresar Un Password",
                    "Aceptar"
                    );
                return;
            }

            this.IsRunning = true;
            this.IsEnabled = false;

            var connetion = await this.apiService.CheckConnection();
            if (!connetion.IsSuccess)
            {
                this.isRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    Lenguages.Error,
                    connetion.Message,
                    Lenguages.Accept
                    );
                return;
            }

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var token = await this.apiService.GetToken(url, this.Email, this.Password);
            if (token == null || string.IsNullOrEmpty(token.AccessToken))
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert("Error", "Usuario o password Incorrectos", "Aceptar");
                return;
            }

            Settings.TokenType = token.TokenType;
            Settings.AccessToken = token.AccessToken;
            Settings.IsRemembered = this.IsRemembered;

            MainViewModel.GetInstance().Products = new ProductsViewModel();
            Application.Current.MainPage = new ProductsPage();

            this.IsRunning = false;
            this.IsEnabled = true;

        }
        public ICommand ForgotPasswordComand
        {
            get
            {
                return new RelayCommand(ForgotPassword);
            }
        }

        private async void ForgotPassword()
        {
            
        }
        #endregion
    }
}

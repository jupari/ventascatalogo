namespace ventascatalogo.ViewModels
{
 
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Xamarin.Forms;
    using Views;

    public class MainViewModel

    {
        #region Propiedades
        public ProductsViewModel Products { get; set; }
        public LoginViewModel Login { get; set; }
        public NewProductViewModel NewProduct { get; set; }
        public ICommand AddProductCommand
        {
            get
            {
                return new RelayCommand(GotoAddProduct);
            }
        }
        public EditProductViewModel EditProduct { get; set; }
        public ObservableCollection<MenuItemViewModel> Menu { get; set; }
        #endregion
        #region Metodos
        private async void GotoAddProduct()
        {
            this.NewProduct = new NewProductViewModel();
            await App.Navigator.PushAsync(new NewProductPage());
        }
        #endregion
        #region Constructor

        public MainViewModel()
        {
            instance = this;
            this.LoadMenu();
        }
        #endregion
        #region singleton
        private static MainViewModel instance;
        public static MainViewModel GetInstance()
        {
            if (instance == null)
            {
                return new MainViewModel();
            }

            return instance;
        }

        #endregion
        #region Metodos
        private void LoadMenu()
        {
            this.Menu = new ObservableCollection<MenuItemViewModel>();
            this.Menu.Add(new MenuItemViewModel
            {
                Icon="ic_info",
                PageName="AboutPage",
                Title="Acerca de"
            });
            this.Menu.Add(new MenuItemViewModel
            {
                Icon = "ic_phonelink_setup",
                PageName = "SetupPage",
                Title = "Configuracion"
            });
            this.Menu.Add(new MenuItemViewModel
            {
                Icon = "ic_exit_to_app",
                PageName = "LoginPage",
                Title = "Cerrar cesion"
            });

        }
        #endregion

    }
}

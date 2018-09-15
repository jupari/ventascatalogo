namespace ventascatalogo.ViewModels
{
    using System;
    using System.Linq;
    using System.Windows.Input;
    using Common.Models;
    using GalaSoft.MvvmLight.Command;
    using Services;
    using Helpers;
    using Views;
    using Xamarin.Forms;

    public class ProductItemViewModel: Product
    {
        #region Atributos
        private ApiService apiService;
        #endregion
        #region Contructores
        public ProductItemViewModel()
        {
            this.apiService = new ApiService();
        }
        #endregion
        #region Command
        public ICommand EditproductCommand
        {
            get
            {
                return new RelayCommand(Editproduct);
            }
        }

        private async void Editproduct()
        {
            MainViewModel.GetInstance().EditProduct = new EditProductViewModel(this);
            await App.Navigator.PushAsync(new EditProductPage());
        }

        public ICommand DeleteproductCommand
        {
            get
            {
                return new RelayCommand(Deleteproduct);
            }

        }

        private async void Deleteproduct()
        {
            var answer = await Application.Current.MainPage.DisplayAlert(
                "Confirmar",
                "Desea Borrar el Producto",
                "SI",
                "No"
                );
            if (!answer)
            {
                return;
            }

            var connetion = await this.apiService.CheckConnection();
            if (!connetion.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Lenguages.Error,
                    connetion.Message,
                    Lenguages.Accept
                    );
                return;
            }

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();
            var response = await this.apiService.Delete(url, prefix, controller, this.ProductId, Settings.TokenType, Settings.AccessToken);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Lenguages.Error,
                    response.Message,
                    Lenguages.Accept
                    );
                return;
            }
            //instanciamos la productViewModel para que ejecute el comando
            var productsViewModel = ProductsViewModel.GetInstance();
            var deleteProduct = productsViewModel.Products.Where(p => p.ProductId == this.ProductId).FirstOrDefault();
            if (deleteProduct!=null)
            {
                productsViewModel.Products.Remove(deleteProduct);
            }
            //productsViewModel.RefreshList();
        }
        #endregion
    }
}

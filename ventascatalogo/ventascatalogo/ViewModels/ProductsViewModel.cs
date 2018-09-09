namespace ventascatalogo.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;
    using System.Windows.Input;
    using Common.Models;
    using GalaSoft.MvvmLight.Command;
    using Services;
    using ventascatalogo.Helpers;
    using Xamarin.Forms;

    public class ProductsViewModel:BaseViewModels
    {
        #region Atributes
        private ObservableCollection<Product> products;
        private ApiService apiService;
        private bool isRefreshing;
        
        #endregion

        #region Properties
        public ObservableCollection<Product> Products
        {
            get { return this.products; }
            set { SetValue(ref this.products, value); }
        }
        public bool IsRefreshing
        {
            get { return this.isRefreshing; }
            set { SetValue(ref this.isRefreshing, value); }
        }
        #endregion
        #region contructor
        public ProductsViewModel()
        {
            this.apiService = new ApiService();
            this.LoadProducts();
        }

        #endregion

        #region Metodos
        private async void LoadProducts()
        {
            this.IsRefreshing = true;

            var connetion = await this.apiService.CheckConnection();
            if (!connetion.IsSuccess)
            {
                this.isRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    connetion.Message,
                    Languages.Accept
                    );
                return;
            }

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix= Application.Current.Resources["UrlPrefix"].ToString();
            var controller= Application.Current.Resources["UrlProductsController"].ToString();
            var response = await this.apiService.GetList<Product>(url, prefix, controller);
            if (!response.IsSuccess)
            {
                this.IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    response.Message,
                  Languages.Accept
                    );
                return;
            }

            var list = (List<Product>)response.Result;
            this.Products = new ObservableCollection<Product>(list);
            this.IsRefreshing = false;
        }
        #endregion
        #region commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(LoadProducts);
            }
        }
        #endregion


    }
}

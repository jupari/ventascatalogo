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
    using Helpers;
    using Xamarin.Forms;
    using System.Linq;
    using System.Threading.Tasks;

    public class ProductsViewModel : BaseViewModels
    {
        #region Atributes
        private string filter;
        private ObservableCollection<ProductItemViewModel> products;
        private ApiService apiService;
        private DataService dataService;
        private bool isRefreshing;

        #endregion
        #region Properties
        public string Filter
        {
            get { return this.filter; }
            set
            {
                this.filter = value;
                this.RefreshList();
            }
        }
        public ObservableCollection<ProductItemViewModel> Products
        {
            get { return this.products; }
            set { SetValue(ref this.products, value); }
        }
        public bool IsRefreshing
        {
            get { return this.isRefreshing; }
            set { SetValue(ref this.isRefreshing, value); }
        }
        public List<Product> MyProducts { get; set; }
        #endregion
        #region contructor
        public ProductsViewModel()
        {
            instance = this;
            this.apiService = new ApiService();
            this.dataService = new DataService();
            this.LoadProducts();
        }

        #endregion
        #region singleton
        private static ProductsViewModel instance;
        public static ProductsViewModel GetInstance()
        {
            if( instance==null)
            {
                return new ProductsViewModel();
            }

            return instance;
        }

        #endregion
        #region Metodos
        private async void LoadProducts()
        {
            this.IsRefreshing = true;

            var connetion = await this.apiService.CheckConnection();
            if (connetion.IsSuccess)
            {
                var answer = await this.LoadProductsFromAPI();
                if (answer)
                {
                    this.SaveProductsToDB();
                }
                //this.isRefreshing = false;
                //await Application.Current.MainPage.DisplayAlert(
                //    Lenguages.Error,
                //    connetion.Message,
                //    Lenguages.Accept
                //    );
                //return;
            }
            else
            {
                await this.LoadProductsFromDB();
            }

            if(this.MyProducts==null) if (this.MyProducts == null || this.MyProducts.Count == 0)
            {
                    this.IsRefreshing = false;
                    await Application.Current.MainPage.DisplayAlert("Error", "No hay Productos", "Acepto");
                    return;
            }

            //var url = Application.Current.Resources["UrlAPI"].ToString();
            //var prefix= Application.Current.Resources["UrlPrefix"].ToString();
            //var controller= Application.Current.Resources["UrlProductsController"].ToString();
            //var response = await this.apiService.GetList<Product>(url, prefix, controller,Settings.TokenType,Settings.AccessToken);
            //if (!response.IsSuccess)
            //{
            //    this.IsRefreshing = false;
            //    await Application.Current.MainPage.DisplayAlert(
            //        Lenguages.Error,
            //        response.Message,
            //        Lenguages.Accept
            //        );
            //    return;
            //}


            //this.MyProducts = (List<Product>)response.Result;
            //this.RefreshList();
            //se convierte la lista que viene de result en un ProductItemViewModel
            var myListProductItemViewModel = this.MyProducts.Select(p => new ProductItemViewModel
            {
                Descripcion=p.Descripcion,
                Price=p.Price,
                Remark=p.Remark,
                IsAvailable=p.IsAvailable,
                PublishOn=p.PublishOn,
                ProductId=p.ProductId,
                ImageArray=p.ImageArray,
                ImagePath=p.ImagePath
            });

            this.Products = new ObservableCollection<ProductItemViewModel>(
                myListProductItemViewModel.OrderBy(p=>p.Descripcion));
            this.IsRefreshing = false;
        }

        private async Task LoadProductsFromDB()
        {
            this.MyProducts = await this.dataService.GetAllProducts();
        }

        private async Task SaveProductsToDB()
        {
            await this.dataService.DeleteAllProducts();
            this.dataService.Insert(this.MyProducts);
        }

        private async Task<bool> LoadProductsFromAPI()
        {
            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();
            var response = await this.apiService.GetList<Product>(url, prefix, controller, Settings.TokenType, Settings.AccessToken);
            if (!response.IsSuccess)
            {
                return false;
            }
            this.MyProducts = (List<Product>)response.Result;
            return true;
        }

        //Refresca la Lista de productos
        public void RefreshList()
        {
            if (string.IsNullOrEmpty(this.Filter))
            {
                var myListProductItemViewModel = this.MyProducts.Select(p => new ProductItemViewModel
                {
                    Descripcion = p.Descripcion,
                    ImageArray = p.ImageArray,
                    ImagePath = p.ImagePath,
                    IsAvailable = p.IsAvailable,
                    Price = p.Price,
                    ProductId = p.ProductId,
                    PublishOn = p.PublishOn,
                    Remark = p.Remark,
                });

                this.Products = new ObservableCollection<ProductItemViewModel>(
                    myListProductItemViewModel.OrderBy(p => p.Descripcion));
            }
            else
            {
                var myListProductItemViewModel = this.MyProducts.Select(p => new ProductItemViewModel
                {
                    Descripcion = p.Descripcion,
                    ImageArray = p.ImageArray,
                    ImagePath = p.ImagePath,
                    IsAvailable = p.IsAvailable,
                    Price = p.Price,
                    ProductId = p.ProductId,
                    PublishOn = p.PublishOn,
                    Remark = p. Remark,
                }).Where(p => p.Descripcion.ToLower().Contains(this.Filter.ToLower())).ToList();

                this.Products = new ObservableCollection<ProductItemViewModel>(
                    myListProductItemViewModel.OrderBy(p => p.Descripcion));
            }
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

        public ICommand SearchCommand
        {
            get
            {
                return new RelayCommand(RefreshList);
            }
        }
        #endregion
    }
}

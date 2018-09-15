namespace ventascatalogo.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Plugin.Media.Abstractions;
    using Common.Models;
    using Helpers;
    using Services;
    using Xamarin.Forms;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Plugin.Media;
    using System.Linq;

    public class EditProductViewModel : BaseViewModels
    {
        #region Atributos
        private Product product;
        private MediaFile file;
        private ApiService apiService;
        private ImageSource imageSource;
        private bool isRunning;
        private bool isEnabled;
        #endregion

        #region Propiedades
        public string Description { get; set; }
        public string Price { get; set; }
        public string Remark { get; set; }

        public Product Product
        {
            get { return this.product; }
            set { SetValue(ref this.product, value); }
        }

        public bool IsRunning
        {
            get { return this.isRunning; }
            set { SetValue(ref this.isRunning, value); }
        }
        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set { SetValue(ref this.isEnabled, value); }
        }
        public ImageSource ImageSource
        {
            get { return this.imageSource; }
            set { SetValue(ref this.imageSource, value); }
        }
        #endregion

        #region Contructores
        public EditProductViewModel(Product product)
        {
            this.product = product;
            this.apiService = new ApiService();
            this.IsEnabled = true;
            this.ImageSource = product.ImageFullPath;
            this.product = product;
        }
        #endregion

        #region Commands
        public ICommand ChangeImageCommand
        {
            get
            {
                return new RelayCommand(ChangeImage);
            }
        }

        private async void ChangeImage()
        {
            await CrossMedia.Current.Initialize();

            var source = await Application.Current.MainPage.DisplayActionSheet(
                "Seleccionar Imagen",
                "Cancelar",
                null,
                "Seleccionar de la Galeria",
                "Nueva Foto");

            if (source == "Cancelar")
            {
                this.file = null;
                return;
            }

            if (source == "Nueva Foto")
            {
                this.file = await CrossMedia.Current.TakePhotoAsync(
                    new StoreCameraMediaOptions
                    {
                        Directory = "Sample",
                        Name = "test.jpg",
                        PhotoSize = PhotoSize.Small,
                    }
                );
            }
            else
            {
                this.file = await CrossMedia.Current.PickPhotoAsync();
            }

            if (this.file != null)
            {
                this.ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = this.file.GetStream();
                    return stream;
                });
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(Save);
            }
        }

        private async void Save()
        {
            if (string.IsNullOrEmpty(this.Product.Descripcion))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",//Lenguages.Error,
                    "Digitar descripcion",//Lenguages.DescriptionError,
                    "Acepto"//Lenguages.Accept
                    );
                return;
            }

            if (this.Product.Price < 0)
            {
                await Application.Current.MainPage.DisplayAlert(
                     "Error",//Lenguages.Error,
                     "El precio debe ser mayor que cero",//Lenguages.PriceError,
                     "Aceptar"//Lenguages.Accept
                     );
            }

            this.IsRunning = true;
            this.IsEnabled = false;

            var connetion = await this.apiService.CheckConnection();
            if (!connetion.IsSuccess)
            {
                this.IsRunning = false;
                this.isEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    Lenguages.Error,
                    connetion.Message,
                    Lenguages.Accept
                    );
                return;
            }

            byte[] imageArray = null;
            if (this.file != null)
            {
                imageArray = Helpers.FilesHelper.ReadFully(this.file.GetStream());
                this.product.ImageArray = imageArray;
            }

             var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();
            var response = await this.apiService.Put(url, prefix, controller, this.product,this.product.ProductId, Settings.TokenType, Settings.AccessToken);
            if (!response.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    "Error",//Lenguages.Error,
                    response.Message,
                    "Aceptar"//Lenguages.Accept
                    );
                return;
            }

            var newProduct = (Product)response.Result;
            var productsviewModel = ProductsViewModel.GetInstance();
            //Se borra de la lista el anterior producto para despues adiccionarlo
            var oldProduct = productsviewModel.MyProducts.Where(p => p.ProductId == this.product.ProductId).FirstOrDefault();
            if (oldProduct != null)
            {
                productsviewModel.MyProducts.Remove(oldProduct);
            }

            productsviewModel.MyProducts.Add(newProduct);
            productsviewModel.RefreshList();

            //igual se debe enviar a insertar a bd productItemviewmodel
            //viewModel.Products.Add(new ProductItemViewModel {
            //    Descripcion = newProduct.Descripcion,
            //    Price = newProduct.Price,
            //    Remark = newProduct.Remark,
            //    IsAvailable = newProduct.IsAvailable,
            //    PublishOn = newProduct.PublishOn,
            //    ProductId = newProduct.ProductId,
            //    ImageArray = newProduct.ImageArray,
            //    ImagePath = newProduct.ImagePath
            //});

            this.IsRunning = false;
            this.IsEnabled = true;

            //volver a la pagina de inicio

            await App.Navigator.PopAsync();
        }
        #endregion

    }
}

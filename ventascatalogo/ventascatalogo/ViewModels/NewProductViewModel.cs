namespace ventascatalogo.ViewModels
{
    using System;
    using System.Windows.Input;
    using Common.Models;
    using GalaSoft.MvvmLight.Command;
    using Helpers;
    using Plugin.Media;
    using Plugin.Media.Abstractions;
    using Services;
    using Xamarin.Forms;

    public class NewProductViewModel:BaseViewModels
    {

        #region Atributos
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
        #region Constructor
        public NewProductViewModel()
        {
            this.apiService = new ApiService();
            this.isEnabled = true;
            this.ImageSource = "imgnodisponible";
        }
        #endregion
        #region Metodos
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
            if (string.IsNullOrEmpty(this.Description))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",//Lenguages.Error,
                    "Digitar descripcion",//Lenguages.DescriptionError,
                    "Acepto"//Lenguages.Accept
                    );
                return;
            }
            if (string.IsNullOrEmpty(this.Price))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",//Lenguages.Error,
                    "Debe digitar un precio",//Lenguages.PriceError,
                    "Aceptar"//Lenguages.Accept
                    );
                return;
            }

            var price = decimal.Parse(this.Price);
            if (price < 0)
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
                imageArray = FilesHelper.ReadFully(this.file.GetStream());
            }

            var product = new Product
            {
                Descripcion = this.Description,
                Price = price,
                Remark = this.Remark,
                ImageArray = imageArray
            };

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();
            var response = await this.apiService.Post(url, prefix, controller,product,Settings.TokenType,Settings.AccessToken);
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

            await Application.Current.MainPage.Navigation.PopAsync();
        }

        #endregion
      }
}

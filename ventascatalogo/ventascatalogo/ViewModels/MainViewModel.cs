namespace ventascatalogo.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class MainViewModel

    {
        #region Propiedades
        public ProductsViewModel Products { get; set; }
        #endregion
        #region Constructor
        public MainViewModel()
        {
            this.Products = new ProductsViewModel();
        }
        #endregion
    }
}

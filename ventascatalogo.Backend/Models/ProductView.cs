namespace ventascatalogo.Backend.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Common.Models;

    public class ProductView:Product
    {
        #region Atributos
        public HttpPostedFileBase ImageFile { get; set; }
        #endregion


    }
}
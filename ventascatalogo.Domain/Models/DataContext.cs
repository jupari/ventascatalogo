namespace ventascatalogo.Domain.Models 
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DataContext : DbContext
    {
        #region Contructores
        //cada vez que se instancie esta clase se conecta automaticamente a la base de datos
        public DataContext():base("DefaultConnection")
        {

        }
        #endregion

    }
}

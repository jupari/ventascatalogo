namespace ventascatalogo.Common.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Descripcion { get; set; }

        public Decimal Price { get; set; }

        public bool IsAvailable { get; set; }

        public DateTime PublishOn { get; set; }
    }
}

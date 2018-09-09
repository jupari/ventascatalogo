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
        [StringLength(50)]
        public string Descripcion { get; set; }

        [DataType(DataType.MultilineText)]
        public string Remark { get; set; }

        [Display(Name ="Image")]
        public string ImagePath { get; set; }

        [DisplayFormat(DataFormatString ="{0:C2}",ApplyFormatInEditMode =false)]
        public Decimal Price { get; set; }

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }


        [Display(Name = "Publish On")]
        [DataType(DataType.Date)]
        public DateTime PublishOn { get; set; }

        public override string ToString()
        {
            return this.Descripcion;
        }
    }
}

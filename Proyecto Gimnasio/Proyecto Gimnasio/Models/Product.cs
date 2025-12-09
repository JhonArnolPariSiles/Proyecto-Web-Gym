using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Proyecto_Gimnasio.Models;

namespace Proyecto_Gimnasio.Models
{
    public class Product : AuditData
    {
        [Key]
        public int IdProduct { get; set; }

        [Required(ErrorMessage = "Name Product is required")]
        [StringLength(200, ErrorMessage = "{0} must be: minimum {2} and maximum {1}", MinimumLength = 4)]
        [Display(Name = "NAME PRODUCT")]
        public string NameProduct { get; set; }

        [Display(Name = "Image")]
        [StringLength(255, ErrorMessage = "Image path cannot exceed {1} characters")]
        public string? Image { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Debe seleccionar una imagen")]
        [Display(Name = "Image File")]
        public IFormFile? ImageFile { get; set; }

        [Required(ErrorMessage = "Stock is required")]
        [Display(Name = "Stock")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be at least 0")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Expiration Date date is required")]
        [Display(Name = "EXPIRATION DATE")]
        [DataType(DataType.Date)]
        public DateTime ExpirationDate { get; set; }
        public int IdCategory { get; set; }

		[ForeignKey("IdCategory")]
		public Category? Category { get; set; }
		public ICollection<SaleDetailsProducts>? SaleDetailsProducts { get; set; }

	}
}

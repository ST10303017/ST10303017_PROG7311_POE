/*
Calwyn Govender
ST10303017
PROG7311
(OpenAI, 2025)
(Troelsen & Japikse, 2022)
*/

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10303017_PROG7311_POE.Models
{
    public class Product
    {
        [Key] 
        public int ProductID { get; set; } // Primary Key

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(50)]
        public string Category { get; set; } // E.g., "Vegetables", "Fruits", "Dairy"

        [Required(ErrorMessage = "Production date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Production Date")]
        public DateTime ProductionDate { get; set; }

        // Foreign Key to ApplicationUser (Farmer)
        [Required]
        public string FarmerID { get; set; }

        [ForeignKey("FarmerID")] 
        public virtual ApplicationUser Farmer { get; set; } 
    }
}
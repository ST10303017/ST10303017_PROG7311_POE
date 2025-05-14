// File: Models/Product.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10303017_PROG7311_POE.Models
{
    public class Product
    {
        [Key] // Explicitly define Primary Key
        public int ProductID { get; set; } // Primary Key (int)

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
        public string FarmerID { get; set; } // Foreign Key (string, to match ApplicationUser.Id)

        [ForeignKey("FarmerID")] // Specifies that FarmerID is the FK for the 'Farmer' navigation property
        public virtual ApplicationUser Farmer { get; set; } // Navigation property
    }
}
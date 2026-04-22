using System.ComponentModel.DataAnnotations;

namespace FanaCRM.ViewModels.ProductVMs
{
    public class ProductEditVM
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [Range(0.01, 999999999)]
        public decimal Price { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}
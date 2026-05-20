using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FanaCRM.Models
{
    public class OpportunityProduct
    {
        public int Id { get; set; }

        [Required]
        public int OpportunityId { get; set; }

        public Opportunity Opportunity { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Required]
        [Range(1, 100000)]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, 999999999)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Optional but VERY useful (not mapped to DB)
        [NotMapped]
        public decimal Total => Quantity * Price;
    }
}
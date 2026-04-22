namespace FanaCRM.ViewModels.ProductVMs
{
    public class ProductDetailsVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        // Optional: show related data count instead of full navigation
        public int OpportunityCount { get; set; }
    }
}
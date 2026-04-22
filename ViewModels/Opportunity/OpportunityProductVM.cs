namespace FanaCRM.ViewModels
{
    public class OpportunityProductVM
    {
        public int ProductId { get; set; }

        public string? ProductName { get; set; } // ✅ nullable

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Total => Quantity * Price;
    }
}
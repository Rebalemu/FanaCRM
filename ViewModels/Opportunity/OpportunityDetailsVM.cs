

namespace FanaCRM.ViewModels
{
    public class OpportunityDetailsVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CompanyName { get; set; }

        public string? ContactName { get; set; }

        public string StageName { get; set; }

        public int Probability { get; set; }

        public decimal Amount { get; set; }

        public DateTime? CloseDate { get; set; }

        public string? AssignedTo { get; set; }

        public List<OpportunityProductVM> Products { get; set; }

    }
}
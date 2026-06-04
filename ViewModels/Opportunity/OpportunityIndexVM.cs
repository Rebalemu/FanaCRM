

namespace FanaCRM.ViewModels
{
    public class OpportunityIndexVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CompanyName { get; set; }

        public string StageName { get; set; }

        public decimal Amount { get; set; }

        public int Probability { get; set; }

        public DateTime? CloseDate { get; set; }

        public string AssignedTo { get; set; }
    }

}
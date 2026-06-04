namespace FanaCRM.ViewModels.Reports
{
    public class PipelineStageVM
    {
        public string? StageName { get; set; }

        public int OpportunityCount { get; set; }

        public decimal TotalValue { get; set; }

        public int Probability { get; set; }
    }
}
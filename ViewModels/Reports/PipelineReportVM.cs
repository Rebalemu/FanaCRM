namespace FanaCRM.ViewModels.Reports
{
    public class PipelineReportVM
    {
        public int TotalOpportunities { get; set; }

        public int OpenOpportunities { get; set; }

        public int WonOpportunities { get; set; }

        public int LostOpportunities { get; set; }

        public decimal TotalPipelineValue { get; set; }

        public decimal WonRevenue { get; set; }

        public decimal LostRevenue { get; set; }

        public decimal WinRate { get; set; }

        public List<PipelineStageVM> StageBreakdown { get; set; } = new();

        public List<WonLostTrendVM> WonLostTrend { get; set; } = new();
    }
}
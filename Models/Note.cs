namespace FanaCRM.Models
{
    public class Note
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public DateTime CreatedDate { get; set; }

        public string UserId { get; set; }

        public int? LeadId { get; set; }

        public int? OpportunityId { get; set; }
    }
}
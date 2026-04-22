namespace FanaCRM.ViewModels
{
    public class CompanyDetailsVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Industry { get; set; }
        public string? Website { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<ContactIndexVM> Contacts { get; set; } = new();
    }
}
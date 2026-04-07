
using Microsoft.AspNetCore.Identity;

namespace FanaCRM.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ICollection<Lead> Leads { get; set; }
    }
}
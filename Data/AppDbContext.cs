
using FanaCRM.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FanaCRM.Data
{
    public class AppDbContext : IdentityDbContext<Users>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<LeadSource> LeadSources { get; set; }
        public DbSet<LeadStatus> LeadStatuses { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Opportunity> Opportunities { get; set; }
        public DbSet<OpportunityProduct> OpportunityProducts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OpportunityStage> OpportunityStages { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityType> ActivityTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Contact>()
                .HasOne(c => c.Company)
                .WithMany(a => a.Contacts)
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OpportunityStage>().HasData(
                new OpportunityStage { Id = 1, Name = "Prospect", Probability = 10 },
                new OpportunityStage { Id = 2, Name = "Qualified", Probability = 30 },
                new OpportunityStage { Id = 3, Name = "Proposal", Probability = 60 },
                new OpportunityStage { Id = 4, Name = "Negotiation", Probability = 80 },
                new OpportunityStage { Id = 5, Name = "Won", Probability = 100 },
                new OpportunityStage { Id = 6, Name = "Lost", Probability = 0 }
              );
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop - Dell XPS 13", Price = 1200.00m, Description = "13-inch ultrabook, 16GB RAM, 512GB SSD", IsActive = true },
                new Product { Id = 2, Name = "Desktop PC - Custom Build", Price = 950.00m, Description = "Ryzen 5, 16GB RAM, 1TB SSD, GTX 1660", IsActive = true },
                new Product { Id = 3, Name = "Wireless Mouse - Logitech", Price = 25.50m, Description = "Ergonomic wireless mouse", IsActive = true },
                new Product { Id = 4, Name = "Mechanical Keyboard", Price = 75.00m, Description = "RGB backlit mechanical keyboard", IsActive = true },
                new Product { Id = 5, Name = "27-inch Monitor - 4K", Price = 300.00m, Description = "Ultra HD IPS display", IsActive = true },
                new Product { Id = 6, Name = "External SSD 1TB", Price = 150.00m, Description = "Portable high-speed storage", IsActive = true },
                new Product { Id = 7, Name = "USB-C Hub", Price = 40.00m, Description = "Multiport adapter with HDMI, USB 3.0", IsActive = true },
                new Product { Id = 8, Name = "Gaming Headset", Price = 60.00m, Description = "Surround sound headset with mic", IsActive = true },
                new Product { Id = 9, Name = "Webcam HD 1080p", Price = 45.00m, Description = "Full HD webcam for streaming and meetings", IsActive = true },
                new Product { Id = 10, Name = "Office Software License", Price = 120.00m, Description = "1-year subscription license", IsActive = true }
             );
            modelBuilder.Entity<Product>()
                    .HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<ActivityType>().HasData(
                    new ActivityType { Id = 1, Name = "Call" },
                    new ActivityType { Id = 2, Name = "Meeting" },
                    new ActivityType { Id = 3, Name = "Email" },
                    new ActivityType { Id = 4, Name = "Task" }
                );
        }

    }
}


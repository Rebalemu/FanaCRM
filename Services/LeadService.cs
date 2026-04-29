using FanaCRM.Data;
using FanaCRM.Models;
using Microsoft.EntityFrameworkCore;

public interface ILeadService
{
    Task<int> ConvertLeadAsync(int leadId);
}
public class LeadService : ILeadService
{
    private readonly AppDbContext _context;

    public LeadService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> ConvertLeadAsync(int leadId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var lead = await _context.Leads
                .FirstOrDefaultAsync(l => l.Id == leadId);

            if (lead == null)
                throw new Exception("Lead not found");

            if (lead.IsConverted)
                throw new Exception("Lead already converted");

            // ✅ BETTER: Check by Status Name instead of magic number
            var status = await _context.LeadStatuses
                .Where(s => s.Id == lead.StatusId)
                .Select(s => s.Name)
                .FirstOrDefaultAsync();

            if (status != "Qualified")
                throw new Exception("Lead must be qualified first");

            // ===============================
            // 1. CHECK OR CREATE COMPANY
            // ===============================
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Name == lead.Company);

            if (company == null)
            {
                company = new Company
                {
                    Name = lead.Company,
                    Phone = lead.Phone ?? "",
                    Address = "Unknown",
                    CreatedDate = DateTime.UtcNow
                };

                _context.Companies.Add(company);
                await _context.SaveChangesAsync(); // ensure ID is generated
            }

            // ===============================
            // 2. CHECK OR CREATE CONTACT
            // ===============================
            var contact = await _context.Contacts
                .FirstOrDefaultAsync(c =>
                    c.Email == lead.Email && c.Email != null);

            if (contact == null)
            {
                contact = new Contact
                {
                    FullName = lead.FullName,
                    Email = lead.Email,
                    Phone = lead.Phone,
                    CompanyId = company.Id,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();
            }

            // ===============================
            // 3. CREATE OPPORTUNITY
            // ===============================
            var opportunity = new Opportunity
            {
                Name = $"Deal with {company.Name}",
                CompanyId = company.Id,
                ContactId = contact.Id,
                StageId = 1, // default stage
                AssignedTo = lead.AssignedTo,
                CloseDate = DateTime.UtcNow.AddDays(30),
                CreatedDate = DateTime.UtcNow
            };

            _context.Opportunities.Add(opportunity);

            // ===============================
            // 4. MARK LEAD AS CONVERTED
            // ===============================
            lead.IsConverted = true;
            _context.Leads.Update(lead);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return opportunity.Id;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
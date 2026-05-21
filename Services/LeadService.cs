using FanaCRM.Data;
using FanaCRM.Models;
using FanaCRM.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public interface ILeadService
{
    Task<int> ConvertLeadAsync(int leadId, string userId);
}

public class LeadService : ILeadService
{
    private readonly AppDbContext _context;
    private readonly ITimelineService _timelineService;

    public LeadService(
        AppDbContext context,
        ITimelineService timelineService)
    {
        _context = context;
        _timelineService = timelineService;
    }

    public async Task<int> ConvertLeadAsync(
        int leadId,
        string userId)
    {
        using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            // =====================================
            // LOAD LEAD
            // =====================================

            var lead = await _context.Leads
                .FirstOrDefaultAsync(l => l.Id == leadId);

            if (lead == null)
                throw new Exception("Lead not found.");

            if (lead.IsConverted)
                throw new Exception("Lead already converted.");

            var statusName = await _context.LeadStatuses
                .Where(s => s.Id == lead.StatusId)
                .Select(s => s.Name)
                .FirstOrDefaultAsync();

            if (statusName != "Qualified")
            {
                throw new Exception(
                    "Only qualified leads can be converted.");
            }

            // =====================================
            // COMPANY
            // =====================================

            var company = await _context.Companies
                .FirstOrDefaultAsync(c =>
                    c.Name == lead.Company);

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
            }

            // =====================================
            // SAVE COMPANY FIRST
            // =====================================

            await _context.SaveChangesAsync();

            // =====================================
            // CONTACT
            // =====================================

            var contact = await _context.Contacts
                .FirstOrDefaultAsync(c =>
                    c.Email == lead.Email &&
                    c.Email != null);

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
            }

            // =====================================
            // SAVE CONTACT FIRST
            // =====================================

            await _context.SaveChangesAsync();

            // =====================================
            // DEFAULT STAGE
            // =====================================

            var defaultStageId = await _context.OpportunityStages
                .OrderBy(s => s.Order)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            // =====================================
            // CREATE OPPORTUNITY
            // =====================================

            var opportunity = new Opportunity
            {
                Name = $"Deal with {company.Name}",

                CompanyId = company.Id,

                ContactId = contact.Id,

                LeadId = lead.Id,

                StageId = defaultStageId,

                AssignedTo = lead.AssignedTo,

                CloseDate = DateTime.UtcNow.AddDays(30),

                CreatedDate = DateTime.UtcNow
            };

            _context.Opportunities.Add(opportunity);

            // save first to generate opportunity id
            await _context.SaveChangesAsync();

            // =====================================
            // LINK LEAD -> OPPORTUNITY
            // =====================================

            lead.OpportunityId = opportunity.Id;

            // =====================================
            // MARK LEAD AS CONVERTED
            // =====================================

            lead.IsConverted = true;

            _context.Leads.Update(lead);

            await _context.SaveChangesAsync();

            // =====================================
            // TIMELINE EVENT
            // =====================================

            await _timelineService.AddEventAsync(
                title: "Lead Converted",

                description:
                    $"{lead.FullName} converted to opportunity.",

                eventType: "Conversion",

                userId: userId,

                leadId: lead.Id,

                opportunityId: opportunity.Id
            );

            await _timelineService.AddEventAsync(
                title: "Opportunity Created",

                description:
                    $"Opportunity created for {company.Name}.",

                eventType: "Opportunity",

                userId: userId,

                leadId: lead.Id,

                opportunityId: opportunity.Id
            );

            // =====================================
            // COMMIT
            // =====================================

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
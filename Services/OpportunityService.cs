using FanaCRM.Data;
using FanaCRM.Models;
using FanaCRM.Services.Interfaces;
using FanaCRM.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FanaCRM.Services.Interfaces
{
    public interface IOpportunityService
    {
        Task<List<OpportunityIndexVM>> GetAllAsync(
            string? search,
            int? stageId);

        Task<OpportunityDetailsVM?> GetDetailsAsync(int id);

        Task<OpportunityFormVM> BuildFormVMAsync(
            OpportunityFormVM vm);

        Task<(bool Success, string? Error)> CreateAsync(
            OpportunityFormVM vm);

        Task<(bool Success, string? Error)> UpdateAsync(
            int id,
            OpportunityFormVM vm);

        Task<bool> DeleteAsync(int id);

        Task<List<SelectListItem>> GetStagesDropdownAsync();
    }

    public class OpportunityService : IOpportunityService
    {
        private readonly AppDbContext _context;
        private readonly ITimelineService _timelineService;

        public OpportunityService(
            AppDbContext context,
            ITimelineService timelineService)
        {
            _context = context;
            _timelineService = timelineService;
        }

        // =====================================================
        // GET ALL
        // =====================================================

        public async Task<List<OpportunityIndexVM>> GetAllAsync(
            string? search,
            int? stageId)
        {
            var query = _context.Opportunities
                .AsNoTracking()
                .Include(o => o.Company)
                .Include(o => o.Stage)
                .Include(o => o.User)
                .AsQueryable();

            // SEARCH

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(o =>
                    o.Name.Contains(search) ||
                    o.Company.Name.Contains(search));
            }

            // FILTER STAGE

            if (stageId.HasValue)
            {
                query = query.Where(o =>
                    o.StageId == stageId.Value);
            }

            return await query
                .OrderByDescending(o => o.CreatedDate)
                .Select(o => new OpportunityIndexVM
                {
                    Id = o.Id,

                    Name = o.Name,

                    CompanyName = o.Company.Name,

                    StageName = o.Stage.Name,

                    Probability = o.Stage.Probability,

                    Amount = o.Amount,

                    CloseDate = o.CloseDate,

                    AssignedTo = o.User.FullName
                })
                .ToListAsync();
        }

        // =====================================================
        // DETAILS
        // =====================================================

        public async Task<OpportunityDetailsVM?> GetDetailsAsync(
            int id)
        {
            var opportunity = await _context.Opportunities
                .AsNoTracking()
                .Include(o => o.Company)
                .Include(o => o.Contact)
                .Include(o => o.Stage)
                .Include(o => o.User)
                .Include(o => o.Products)
                    .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (opportunity == null)
                return null;

            var upcomingActivities = await _context.Activities
                .AsNoTracking()
                .Include(a => a.ActivityStatus)
                .Where(a =>
                    a.OpportunityId == opportunity.Id &&
                    !a.IsCompleted &&
                    a.ActivityStatus.Name != "Cancelled")
                .OrderBy(a => a.DueDate)
                .Select(a => new ActivityWidgetVM
                {
                    Id = a.Id,

                    Subject = a.Subject,

                    Type = a.ActivityType.Name,

                    Status = a.ActivityStatus.Name,

                    DueDate = a.DueDate,

                    IsOverdue =
                        a.DueDate.HasValue &&
                        a.DueDate.Value.Date < DateTime.Today
                })
                .ToListAsync();

            var timeline = await _timelineService
                .GetOpportunityTimelineAsync(opportunity.Id);

            return new OpportunityDetailsVM
            {
                Id = opportunity.Id,

                Name = opportunity.Name,

                CompanyId = opportunity.CompanyId,

                CompanyName = opportunity.Company.Name,

                ContactId = opportunity.ContactId,

                ContactName = opportunity.Contact?.FullName,

                StageId = opportunity.StageId,

                StageName = opportunity.Stage.Name,

                Probability = opportunity.Stage.Probability,

                Amount = opportunity.Amount,

                CloseDate = opportunity.CloseDate,

                AssignedToId = opportunity.AssignedTo,

                AssignedToName = opportunity.User.FullName,

                LossReason = opportunity.LossReason,

                Products = opportunity.Products
                    .Select(p => new OpportunityProductVM
                    {
                        ProductId = p.ProductId,

                        ProductName = p.Product.Name,

                        Quantity = p.Quantity,

                        Price = p.Price
                    })
                    .ToList(),

                UpcomingActivities = upcomingActivities,

                Timeline = timeline
            };
        }

        // =====================================================
        // CREATE
        // =====================================================

        public async Task<(bool Success, string? Error)>
            CreateAsync(OpportunityFormVM vm)
        {
            // VALIDATION

            var validation = await ValidateOpportunityAsync(vm);

            if (!validation.Success)
                return validation;

            var opportunity = new Opportunity
            {
                CompanyId = vm.CompanyId,

                ContactId = vm.ContactId,

                Name = vm.Name,

                StageId = vm.StageId,

                CloseDate = vm.CloseDate,

                AssignedTo = vm.AssignedTo,

                LossReason = vm.LossReason,

                CreatedDate = DateTime.UtcNow,

                Amount = CalculateAmount(vm.Products)
            };

            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            // PRODUCTS

            await SyncProductsAsync(
                opportunity.Id,
                vm.Products);

            // STAGE HISTORY

            await TrackStageHistoryAsync(
                opportunity.Id,
                vm.StageId,
                vm.AssignedTo);

            // TIMELINE

            await _timelineService.AddEventAsync(
                title: "Opportunity Created",

                description:
                    $"{opportunity.Name} created.",

                eventType: "Opportunity",

                userId: vm.AssignedTo,

                opportunityId: opportunity.Id
            );

            await _context.SaveChangesAsync();

            return (true, null);
        }

        // =====================================================
        // UPDATE
        // =====================================================

        public async Task<(bool Success, string? Error)>
            UpdateAsync(
                int id,
                OpportunityFormVM vm)
        {
            var opportunity = await _context.Opportunities
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (opportunity == null)
            {
                return (false, "Opportunity not found.");
            }

            // VALIDATION

            var validation = await ValidateOpportunityAsync(vm);

            if (!validation.Success)
                return validation;

            var oldStageId = opportunity.StageId;

            // UPDATE

            opportunity.CompanyId = vm.CompanyId;

            opportunity.ContactId = vm.ContactId;

            opportunity.Name = vm.Name;

            opportunity.StageId = vm.StageId;

            opportunity.CloseDate = vm.CloseDate;

            opportunity.AssignedTo = vm.AssignedTo;

            opportunity.LossReason = vm.LossReason;

            // PRODUCTS

            await SyncProductsAsync(id, vm.Products);

            // AMOUNT

            opportunity.Amount =
                CalculateAmount(vm.Products);

            await _context.SaveChangesAsync();

            // TRACK STAGE CHANGE

            if (oldStageId != vm.StageId)
            {
                await TrackStageHistoryAsync(
                    opportunity.Id,
                    vm.StageId,
                    vm.AssignedTo);

                var stage = await _context
                    .OpportunityStages
                    .FindAsync(vm.StageId);

                await _timelineService.AddEventAsync(
                    title: "Stage Changed",

                    description:
                        $"{opportunity.Name} moved to {stage?.Name}.",

                    eventType: "Opportunity",

                    userId: vm.AssignedTo,

                    opportunityId: opportunity.Id
                );
            }

            // GENERAL UPDATE EVENT

            await _timelineService.AddEventAsync(
                title: "Opportunity Updated",

                description:
                    $"{opportunity.Name} updated.",

                eventType: "Opportunity",

                userId: vm.AssignedTo,

                opportunityId: opportunity.Id
            );

            return (true, null);
        }

        // =====================================================
        // DELETE
        // =====================================================

        public async Task<bool> DeleteAsync(int id)
        {
            var opportunity = await _context.Opportunities
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (opportunity == null)
                return false;

            // TIMELINE

            await _timelineService.AddEventAsync(
                title: "Opportunity Deleted",

                description:
                    $"{opportunity.Name} deleted.",

                eventType: "Opportunity",

                userId: opportunity.AssignedTo,

                opportunityId: opportunity.Id
            );

            // DELETE PRODUCTS

            _context.OpportunityProducts
                .RemoveRange(opportunity.Products);

            // DELETE OPPORTUNITY

            _context.Opportunities.Remove(opportunity);

            await _context.SaveChangesAsync();

            return true;
        }

        // =====================================================
        // BUILD FORM VM
        // =====================================================

        public async Task<OpportunityFormVM>
            BuildFormVMAsync(OpportunityFormVM vm)
        {
            vm.Companies = await _context.Companies
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),

                    Text = c.Name
                })
                .ToListAsync();

            vm.Contacts = await _context.Contacts
                .OrderBy(c => c.FullName)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),

                    Text = c.FullName
                })
                .ToListAsync();

            vm.Stages = await _context.OpportunityStages
                .OrderBy(s => s.Order)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),

                    Text = s.Name
                })
                .ToListAsync();

            vm.Users = await _context.Users
                .OrderBy(u => u.FullName)
                .Select(u => new SelectListItem
                {
                    Value = u.Id,

                    Text = u.FullName
                })
                .ToListAsync();

            vm.ProductsData = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();

            vm.LostStageIds = await _context
                .OpportunityStages
                .Where(s =>
                    s.IsClosed &&
                    !s.IsWon)
                .Select(s => s.Id)
                .ToListAsync();

            return vm;
        }

        // =====================================================
        // STAGES DROPDOWN
        // =====================================================

        public async Task<List<SelectListItem>>
            GetStagesDropdownAsync()
        {
            return await _context.OpportunityStages
                .OrderBy(s => s.Order)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),

                    Text = s.Name
                })
                .ToListAsync();
        }

        // =====================================================
        // VALIDATION
        // =====================================================

        private async Task<(bool Success, string? Error)>
            ValidateOpportunityAsync(
                OpportunityFormVM vm)
        {
            var stage = await _context
                .OpportunityStages
                .FirstOrDefaultAsync(s =>
                    s.Id == vm.StageId);

            if (stage == null)
            {
                return (false, "Invalid stage.");
            }

            // LOST VALIDATION

            if (stage.IsClosed && !stage.IsWon)
            {
                if (string.IsNullOrWhiteSpace(vm.LossReason))
                {
                    return (
                        false,
                        "Loss reason is required."
                    );
                }
            }

            // PRODUCT VALIDATION

            if (vm.Products == null ||
                !vm.Products.Any())
            {
                return (
                    false,
                    "At least one product is required."
                );
            }

            // INVALID QUANTITY

            if (vm.Products.Any(p => p.Quantity <= 0))
            {
                return (
                    false,
                    "Quantity must be greater than zero."
                );
            }

            // INVALID PRICE

            if (vm.Products.Any(p => p.Price < 0))
            {
                return (
                    false,
                    "Price cannot be negative."
                );
            }

            return (true, null);
        }

        // =====================================================
        // SYNC PRODUCTS
        // =====================================================

        private async Task SyncProductsAsync(
            int opportunityId,
            List<OpportunityProductVM> products)
        {
            var existingProducts = await _context
                .OpportunityProducts
                .Where(x =>
                    x.OpportunityId == opportunityId)
                .ToListAsync();

            _context.OpportunityProducts
                .RemoveRange(existingProducts);

            foreach (var p in products)
            {
                _context.OpportunityProducts
                    .Add(new OpportunityProduct
                    {
                        OpportunityId = opportunityId,

                        ProductId = p.ProductId,

                        Quantity = p.Quantity,

                        Price = p.Price
                    });
            }
        }

        // =====================================================
        // CALCULATE AMOUNT
        // =====================================================

        private decimal CalculateAmount(
            List<OpportunityProductVM> products)
        {
            if (products == null ||
                !products.Any())
            {
                return 0;
            }

            return products.Sum(x =>
                x.Quantity * x.Price);
        }

        // =====================================================
        // TRACK STAGE HISTORY
        // =====================================================

        private async Task TrackStageHistoryAsync(
            int opportunityId,
            int stageId,
            string userId)
        {
            var currentStage = await _context
                .Set<OpportunityStageHistory>()
                .FirstOrDefaultAsync(h =>
                    h.OpportunityId == opportunityId &&
                    h.ExitedAt == null);

            // CLOSE CURRENT STAGE

            if (currentStage != null)
            {
                currentStage.ExitedAt =
                    DateTime.UtcNow;
            }

            // NEW STAGE ENTRY

            _context.Add(new OpportunityStageHistory
            {
                OpportunityId = opportunityId,

                StageId = stageId,

                EnteredAt = DateTime.UtcNow,

                ChangedByUserId = userId
            });

            await _context.SaveChangesAsync();
        }
    }
}
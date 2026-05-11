using FanaCRM.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using FanaCRM.Data;
using FanaCRM.Models;
using Microsoft.EntityFrameworkCore;

namespace FanaCRM.Services.Interfaces
{
    public interface IOpportunityService
    {
        Task<List<OpportunityIndexVM>> GetAllAsync(string? search, int? stageId);

        Task<OpportunityDetailsVM?> GetDetailsAsync(int id);

        Task<OpportunityFormVM> BuildFormVMAsync(OpportunityFormVM vm);

        Task<(bool Success, string? Error)> CreateAsync(OpportunityFormVM vm);

        Task<(bool Success, string? Error)> UpdateAsync(int id, OpportunityFormVM vm);

        Task<bool> DeleteAsync(int id);

        Task<List<SelectListItem>> GetStagesDropdownAsync();
    }

    public class OpportunityService : IOpportunityService
    {
        private readonly AppDbContext _context;

        public OpportunityService(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL
        // =========================
        public async Task<List<OpportunityIndexVM>> GetAllAsync(string? search, int? stageId)
        {
            var query = _context.Opportunities
                .AsNoTracking()
                .Include(o => o.Company)
                .Include(o => o.Stage)
                .Include(o => o.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(o =>
                    o.Name.Contains(search) ||
                    o.Company.Name.Contains(search));
            }

            if (stageId.HasValue)
            {
                query = query.Where(o => o.StageId == stageId);
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

        // =========================
        // DETAILS
        // =========================
        public async Task<OpportunityDetailsVM?> GetDetailsAsync(int id)
        {
            var o = await _context.Opportunities
                .AsNoTracking()
                .Include(x => x.Company)
                .Include(x => x.Contact)
                .Include(x => x.Stage)
                .Include(x => x.User)
                .Include(x => x.Products)
                    .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (o == null)
                return null;

            return new OpportunityDetailsVM
            {
                Id = o.Id,
                Name = o.Name,

                CompanyId = o.CompanyId,
                CompanyName = o.Company.Name,

                ContactId = o.ContactId,
                ContactName = o.Contact?.FullName,

                StageId = o.StageId,
                StageName = o.Stage.Name,
                Probability = o.Stage.Probability,

                Amount = o.Amount,
                CloseDate = o.CloseDate,

                AssignedToId = o.AssignedTo,
                AssignedToName = o.User.FullName,

                LossReason = o.LossReason,

                Products = o.Products.Select(p => new OpportunityProductVM
                {
                    ProductId = p.ProductId,
                    ProductName = p.Product.Name,
                    Quantity = p.Quantity,
                    Price = p.Price
                }).ToList()
            };
        }

        // =========================
        // CREATE
        // =========================
        public async Task<(bool Success, string? Error)> CreateAsync(OpportunityFormVM vm)
        {
            var validation = await ValidateClosingStageAsync(vm);

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

                // ✅ CALCULATE DIRECTLY
                Amount = CalculateAmount(vm.Products)
            };

            _context.Opportunities.Add(opportunity);

            // ✅ SAVE FIRST TO GET ID
            await _context.SaveChangesAsync();

            // ✅ NOW SAVE PRODUCTS
            await SyncProductsAsync(opportunity.Id, vm.Products);

            await TrackStageHistoryAsync(
                opportunity.Id,
                vm.StageId,
                vm.AssignedTo);

            await _context.SaveChangesAsync();

            return (true, null);
        }
        // =========================
        // UPDATE
        // =========================
        public async Task<(bool Success, string? Error)> UpdateAsync(int id, OpportunityFormVM vm)
        {
            var opportunity = await _context.Opportunities
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (opportunity == null)
                return (false, "Opportunity not found");


            // LOST VALIDATION
            var validation = await ValidateClosingStageAsync(vm);

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


            await SyncProductsAsync(id, vm.Products);

            // ✅ UPDATE AMOUNT DIRECTLY
            opportunity.Amount = CalculateAmount(vm.Products);

            await _context.SaveChangesAsync();
            // TRACK HISTORY
            if (oldStageId != vm.StageId)
            {
                await TrackStageHistoryAsync(
                    id,
                    vm.StageId,
                    vm.AssignedTo);
            }

            return (true, null);
        }

        // =========================
        // DELETE
        // =========================
        public async Task<bool> DeleteAsync(int id)
        {
            var opportunity = await _context.Opportunities
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (opportunity == null)
                return false;

            _context.OpportunityProducts.RemoveRange(opportunity.Products);

            _context.Opportunities.Remove(opportunity);

            await _context.SaveChangesAsync();

            return true;
        }

        // =========================
        // BUILD FORM VM
        // =========================
        public async Task<OpportunityFormVM> BuildFormVMAsync(OpportunityFormVM vm)
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
                .OrderBy(s => s.Name)
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

            vm.LostStageIds = await _context.OpportunityStages
                .Where(s => s.IsClosed && !s.IsWon)
                .Select(s => s.Id)
                .ToListAsync();

            return vm;
        }

        // =========================
        // STAGES DROPDOWN
        // =========================
        public async Task<List<SelectListItem>> GetStagesDropdownAsync()
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

        // =========================
        // HELPERS
        // =========================


        private async Task<(bool Success, string? Error)> ValidateClosingStageAsync(OpportunityFormVM vm)
        {
            var stage = await _context.OpportunityStages
                .FindAsync(vm.StageId);

            if (stage == null)
                return (false, "Invalid stage");

            if (stage.IsClosed && !stage.IsWon)
            {
                if (string.IsNullOrWhiteSpace(vm.LossReason))
                {
                    return (false,
                        "Loss reason is required when opportunity is lost");
                }
            }

            return (true, null);
        }

        private async Task SyncProductsAsync(
            int opportunityId,
            List<OpportunityProductVM> products)
        {
            var existingProducts = await _context.OpportunityProducts
                .Where(x => x.OpportunityId == opportunityId)
                .ToListAsync();

            _context.OpportunityProducts.RemoveRange(existingProducts);

            foreach (var p in products)
            {
                _context.OpportunityProducts.Add(new OpportunityProduct
                {
                    OpportunityId = opportunityId,
                    ProductId = p.ProductId,
                    Quantity = p.Quantity,
                    Price = p.Price
                });
            }
        }

        // private async Task UpdateAmountAsync(int opportunityId)
        // {
        //     var total = await _context.OpportunityProducts
        //         .Where(x => x.OpportunityId == opportunityId)
        //         .SumAsync(x => x.Quantity * x.Price);

        //     var opportunity = await _context.Opportunities
        //         .FindAsync(opportunityId);

        //     if (opportunity == null)
        //         return;

        //     opportunity.Amount = total;
        // }
        private decimal CalculateAmount(List<OpportunityProductVM> products)
        {
            if (products == null || !products.Any())
                return 0;

            return products.Sum(x => x.Quantity * x.Price);
        }

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

            if (currentStage != null)
            {
                currentStage.ExitedAt = DateTime.UtcNow;
            }

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
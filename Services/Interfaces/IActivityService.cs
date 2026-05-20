using FanaCRM.ViewModels;

namespace FanaCRM.Services.Interfaces
{
    public interface IActivityService
    {
        Task<List<ActivityIndexVM>> GetAllAsync();

        Task<ActivityDetailsVM?> GetDetailsAsync(int id);

        Task<ActivityFormVM> GetCreateVMAsync();

        Task CreateAsync(ActivityFormVM vm, string userId);

        Task<ActivityFormVM?> GetEditVMAsync(int id);

        Task UpdateAsync(ActivityFormVM vm, string userId);

        Task CompleteAsync(int id);

        Task CancelAsync(int id);
        Task<ActivityDashboardVM> GetDashboardAsync(string userId);
    }
}
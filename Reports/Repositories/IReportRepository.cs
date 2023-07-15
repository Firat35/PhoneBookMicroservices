using Shared;

namespace Reports.Repositories
{
    public interface IReportRepository
    {
        Task<List<Report>> GetAllAsync();
        Task<Report> GetByIdAsync(string id);
        Task<Report> AddAsync(Report report);
    }
}

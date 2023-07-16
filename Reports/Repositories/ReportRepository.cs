
using MongoDB.Bson;
using MongoDB.Driver;
using Reports.Repositories;
using Shared;

namespace Reports.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly IMongoCollection<Report> _reportCollection;
        public ReportRepository()
        {

        }
        public ReportRepository(IMongoDatabase mongoDatabase)
        {
            _reportCollection = mongoDatabase.GetCollection<Report>("reports");
        }
        public async Task<List<Report>> GetAllAsync()
        {
            return await _reportCollection.Find(_ => true).ToListAsync(); ;
        }
        

        public async Task<Report> GetByIdAsync(string id)
        {
            return await _reportCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Report> AddAsync(Report report)
        {
            await _reportCollection.InsertOneAsync(report);

            return report;
        }
        public async Task UpdateAsync(Report product)
        {
            await _reportCollection.FindOneAndReplaceAsync(x => x.Id == product.Id, product);
        }


    }
}

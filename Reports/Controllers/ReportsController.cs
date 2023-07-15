using Microsoft.AspNetCore.Mvc;

using Reports.Repositories;
using Reports.Services;

using Shared;

namespace Reports.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportsController : ControllerBase
    {
        //private static List<Report> _reports = new List<Report>();
        private readonly IReportRepository _reportRepository;
        private readonly RabbitMQPublisher _rabbitMQPublisher;

        public ReportsController(RabbitMQPublisher rabbitMQPublisher, IReportRepository reportRepository)
        {
            _rabbitMQPublisher = rabbitMQPublisher;
            _reportRepository = reportRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetReports()
        {
            return Ok(await _reportRepository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReport(Guid id)
        {
            var report = await _reportRepository.GetByIdAsync(id.ToString());
            if (report == null)
                return NotFound();

            return Ok(report);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport()
        {
            var report = new Report();
            //report.Id = Guid.NewGuid().ToString();
            report.RequestedDate = DateTime.Now;
            report.Status = nameof(ReportStatus.Creating);

            // Raporun oluþturulmasý iþlemini gerçekleþtir (asenkron olarak)
            _rabbitMQPublisher.Publish(report: new Report() { Id = report.Id });


            await _reportRepository.AddAsync(report);

            return CreatedAtAction(nameof(GetReport), new { id = report.Id }, new { id = report.Id });
        }
    }
}
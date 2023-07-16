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
        private readonly IReportRepository _reportRepository;
        private readonly RabbitMQPublisher _rabbitMQPublisher;
        private readonly RabbitMQSubscriber _rabbitMQSubscriber;

        public ReportsController(RabbitMQPublisher rabbitMQPublisher, IReportRepository reportRepository, RabbitMQSubscriber rabbitMQSubscriber)
        {
            _rabbitMQPublisher = rabbitMQPublisher;
            _reportRepository = reportRepository;
            _rabbitMQSubscriber = rabbitMQSubscriber;
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
            report.RequestedDate = DateTime.Now;
            report.Status = nameof(ReportStatus.Creating);

            await _reportRepository.AddAsync(report);

            _rabbitMQSubscriber.Subscribe();

            _rabbitMQPublisher.Publish(report: new Report() { Id = report.Id });

            return CreatedAtAction(nameof(GetReport), new { id = report.Id }, new { id = report.Id });
        }
    }
}
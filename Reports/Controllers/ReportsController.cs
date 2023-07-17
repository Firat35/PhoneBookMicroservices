using Microsoft.AspNetCore.Mvc;

using Reports.Models;
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
            var reports = await _reportRepository.GetAllAsync();
            var reportDtos = new List<ReportDto>();
            reports.ToList().ForEach(x =>
            {
                var newReportDto = new ReportDto { Id = x.Id, Status = x.Status, RequestedDate = x.RequestedDate };
                reportDtos.Add(newReportDto);
            });
            return Ok(reportDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReport(string id)
        {
            var report = await _reportRepository.GetByIdAsync(id);
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
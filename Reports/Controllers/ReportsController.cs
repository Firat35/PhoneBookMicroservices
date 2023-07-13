using Microsoft.AspNetCore.Mvc;

namespace Reports.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportsController : ControllerBase
    {
        private static List<Report> _reports = new List<Report>();

        [HttpGet]
        public IActionResult GetReports()
        {
            return Ok(_reports);
        }

        [HttpGet("{id}")]
        public IActionResult GetReport(Guid id)
        {
            var report = _reports.Find(r => r.Id == id);
            if (report == null)
                return NotFound();

            return Ok(report);
        }

        [HttpPost]
        public IActionResult CreateReport([FromBody] Report report)
        {
            report.Id = Guid.NewGuid();
            report.RequestedDate = DateTime.UtcNow;
            report.Status = "Hazýrlanýyor";

            // Raporun oluþturulmasý iþlemini gerçekleþtir (asenkron olarak)

            report.Status = "Tamamlandý";

            _reports.Add(report);

            return CreatedAtAction(nameof(GetReport), new { id = report.Id }, report);
        }
    }
}
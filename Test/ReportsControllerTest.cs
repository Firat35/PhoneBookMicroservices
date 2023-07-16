using Microsoft.AspNetCore.Mvc;

using Moq;

using People.Models;

using Reports.Controllers;
using Reports.Repositories;
using Reports.Services;

using Shared;

namespace Test
{
    
    public class ReportsControllerTest
    {
        private readonly Mock<IReportRepository> _mockReportRepo;
        private readonly Mock<RabbitMQPublisher> _mockpublisher;
        private readonly Mock<RabbitMQSubscriber> _mocksubscriber;
        private readonly ReportsController _controller;
        private List<Report> reports = new List<Report>();
        public ReportsControllerTest()
        {
            _mockReportRepo = new Mock<IReportRepository>();
            _mockpublisher = new Mock<RabbitMQPublisher>();
            _mocksubscriber = new Mock<RabbitMQSubscriber>();
            _controller = new ReportsController(_mockpublisher.Object, _mockReportRepo.Object, _mocksubscriber.Object);
            reports = new List<Report>() { 
                new Report() { Id = "64b3c8ffff72594d3c1a8398", Status = "Creating"},
                new Report() { Id = "64b3de8de145ee1db307f0be", Status = "Completed"}
            };

        }

        [Fact]
        public async void GetReport_ActionExecutes_ReturnOkResultWithReport()
        {
            _mockReportRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(reports);

            var result = await _controller.GetReports();

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnReports = Assert.IsAssignableFrom<IEnumerable<Report>>(okResult.Value);

            Assert.Equal<int>(2, returnReports.ToList().Count);
        }

        [Theory]
        [InlineData("64b3dbbbbc5a068ba96781af")]
        public async void GetReportn_IdInValid_ReturnNotFound(string reportId)
        {
            Report report = null;

            _mockReportRepo.Setup(x => x.GetByIdAsync(reportId)).ReturnsAsync(report);

            var result = await _controller.GetReport(reportId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData("64b3c8ffff72594d3c1a8398")]
        [InlineData("64b3de8de145ee1db307f0be")]
        public async void GetPerson_IdValid_ReturnOkResult(string reportId)
        {
            var report = reports.First(x => x.Id == reportId);

            _mockReportRepo.Setup(x => x.GetByIdAsync(reportId)).ReturnsAsync(report);

            var result = await _controller.GetReport(reportId);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnPerson = Assert.IsType<Report>(okResult.Value);

            Assert.Equal(reportId, returnPerson.Id);
            Assert.Equal(report.Status, returnPerson.Status);
        }
    }
}

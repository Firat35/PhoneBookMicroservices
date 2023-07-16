using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Shared;
using System.Text.Json;
using Reports.Repositories;

namespace Reports.Services
{
    public class RabbitMQSubscriber
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly IReportRepository _reportRepository;

        public RabbitMQSubscriber()
        {

        }
        public RabbitMQSubscriber(ConnectionFactory connectionFactory,
            IReportRepository reportRepository)
        {
            _connectionFactory = connectionFactory;
            _reportRepository = reportRepository;
        }
        public void Subscribe()
        {
            //var factory = new ConnectionFactory();
            //factory.Uri = new Uri("amqps://ncocubce:Zlv-eKkXTpI1PAafqYwDDXDyO9yPkz62@shrimp.rmq.cloudamqp.com/ncocubce");

            var connection = _connectionFactory.CreateConnection();

            var channel = connection.CreateModel();
            channel.ExchangeDeclare("report-header-exchange", durable: true, type: ExchangeType.Headers);

            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);

            var queueName = channel.QueueDeclare().QueueName;

            Dictionary<string, object> headers = new Dictionary<string, object>
            {
                { "report", "location" },
                { "x-match", "any" }
            };

            channel.QueueBind(queueName, "report-header-exchange", String.Empty, headers);

            channel.BasicConsume(queueName, false, consumer);

            consumer.Received += async (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Report report = JsonSerializer.Deserialize<Report>(message);

                var existReport =  await _reportRepository.GetByIdAsync(report.Id);
                if (report == null)
                    return;

                existReport.Status= report.Status;
                existReport.locations = report.locations;
                await _reportRepository.UpdateAsync(existReport);


                channel.BasicAck(e.DeliveryTag, false);
            };

        }
    }
}

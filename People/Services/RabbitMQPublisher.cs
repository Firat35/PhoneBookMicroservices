using RabbitMQ.Client;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Text.Json;
using System.Threading.Channels;
using Shared;
using System.Text;

namespace People.Services
{
    public class RabbitMQPublisher
    {
        //private readonly RabbitMQClientService _rabbitMQClientService;
        private readonly ConnectionFactory _connectionFactory;

        public RabbitMQPublisher(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Publish(Report report)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://ncocubce:Zlv-eKkXTpI1PAafqYwDDXDyO9yPkz62@shrimp.rmq.cloudamqp.com/ncocubce");

            using var connection = factory.CreateConnection();


            var channel = connection.CreateModel();

            channel.ExchangeDeclare("report-header-exchange", durable: true, type: ExchangeType.Headers);


            Dictionary<string, object> headers = new Dictionary<string, object>
            {
                { "report", "location" }
            };

            var properties = channel.CreateBasicProperties();
            properties.Headers = headers;
            properties.Persistent = true;

            var reportJsonString = JsonSerializer.Serialize(report);

            channel.BasicPublish("report-header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes(reportJsonString));
        }
    }
}

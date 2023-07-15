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

        //public RabbitMQPublisher(RabbitMQClientService rabbitMQClientService)
        //{
        //    _rabbitMQClientService = rabbitMQClientService;
        //}

        public void Publish(Report report)
        {
            //var factory = new ConnectionFactory();
            //factory.Uri = new Uri("amqps://uhshoatb:4tRfDsemduk6BCrsZaIvfQgOhLsMtf-t@fish.rmq.cloudamqp.com/uhshoatb");

            using var connection = _connectionFactory.CreateConnection();


            var channel = connection.CreateModel();

            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);


            Dictionary<string, object> headers = new Dictionary<string, object>
            {
                { "report", "location" }
            };

            var properties = channel.CreateBasicProperties();
            properties.Headers = headers;
            properties.Persistent = true;


            //var product = new Report { Id = 1, Name = "Kalem", Price = 100, Stock = 10 };

            var productJsonString = JsonSerializer.Serialize(report);


            //channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes(productJsonString));

            //Console.WriteLine("mesaj gönderilmiştir");
        }
    }
}

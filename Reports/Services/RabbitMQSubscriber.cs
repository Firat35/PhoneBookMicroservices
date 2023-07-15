using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Shared;
using System.Text.Json;

namespace Reports.Services
{
    public class RabbitMQSubscriber
    {
        private readonly ConnectionFactory _connectionFactory;

        public RabbitMQSubscriber(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public void Subscribe()
        {
            //var factory = new ConnectionFactory();
            //factory.Uri = new Uri("amqps://uhshoatb:4tRfDsemduk6BCrsZaIvfQgOhLsMtf-t@fish.rmq.cloudamqp.com/uhshoatb");

            using var connection = _connectionFactory.CreateConnection();

            var channel = connection.CreateModel();
            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);

            var queueName = channel.QueueDeclare().QueueName;

            Dictionary<string, object> headers = new Dictionary<string, object>
            {
                { "report", "location" },
                { "x-match", "any" }
            };


            channel.QueueBind(queueName, "header-exchange", String.Empty, headers);

            channel.BasicConsume(queueName, false, consumer);


            Console.WriteLine("Logları dinleniyor...");

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Report product = JsonSerializer.Deserialize<Report>(message);

                //Thread.Sleep(1500);

                //Console.WriteLine($"Gelen Mesaj: {product.Id}-{product.Name}-{product.Price}-{product.Stock}");



                channel.BasicAck(e.DeliveryTag, false);
            };



        }
    }
}

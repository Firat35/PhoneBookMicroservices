using FileCreateWorkerService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using People.Models;
using People.Services;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using Shared;

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FileCreateWorkerService
{
    public class ReportProcessBackgroundService : BackgroundService
    {
        private readonly ILogger<ReportProcessBackgroundService> _logger;

        private readonly RabbitMQClientService _rabbitMQClientService;

        private readonly IServiceProvider _serviceProvider;

        private IModel _channel;

        private static List<Person> _people = new List<Person>();

        private readonly RabbitMQPublisher _rabbitMQPublisher;
        public ReportProcessBackgroundService(
            ILogger<ReportProcessBackgroundService> logger,
            RabbitMQClientService rabbitMQClientService,
            IServiceProvider serviceProvider,
            RabbitMQPublisher rabbitMQPublisher)
        {
            _logger = logger;
            _rabbitMQClientService = rabbitMQClientService;
            _serviceProvider = serviceProvider;
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {

         _channel=_rabbitMQClientService.Connect();
            _channel.BasicQos(0, 1, false);

            return base.StartAsync(cancellationToken);
        }

        protected override  Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var consumer = new AsyncEventingBasicConsumer(_channel);

            _channel.BasicConsume(RabbitMQClientService.QueueName, false, consumer);
          
            consumer.Received += Consumer_Received;

            return Task.CompletedTask;
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            await Task.Delay(5000);


            var report = JsonSerializer.Deserialize<Report>(Encoding.UTF8.GetString(@event.Body.ToArray()));

            var locations = new List<Location>();

            _people.ForEach(x =>
            {   
                x.ContactInfos.ForEach(y =>
                {
                    if(y.InfoType == "location")
                    {
                        if(!locations.Any(z => z.name == y.InfoContent))
                        {
                            locations.Add(new Location() { name = y.InfoContent });
                        }
                    }

                });
            });

            locations.ForEach(x => {
                x.PersonCount =  _people.Count(y => y.ContactInfos.Any(z => z.InfoType == "location" && z.InfoContent == x.name));
                var phoneCount = 0;
                _people.ForEach(y => {
                    if (y.ContactInfos.Any(z => z.InfoType == "location" && z.InfoContent == x.name))
                        phoneCount += y.ContactInfos.Count(z => z.InfoType == "phoneNumber");
                 });
                x.PhoneNumberCount = phoneCount;
            });

            report.locations = locations;

            _rabbitMQPublisher.Publish(report);

            _channel.BasicAck(@event.DeliveryTag, false);

        }

    }
}

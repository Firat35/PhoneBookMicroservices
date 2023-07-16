using MongoDB.Driver;

using RabbitMQ.Client;

using Reports.Models;
using Reports.Repositories;
using Reports.Services;

namespace Reports
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.Configure<MongoDBSettings>(
               builder.Configuration.GetSection("MongoDBSettings")
           );
            builder.Services.AddSingleton(options => {
                var settings = builder.Configuration.GetSection("MongoDBSettings").Get<MongoDBSettings>();
                var client = new MongoClient(settings.ConnectionString);
                return client.GetDatabase(settings.DatabaseName);
            });

            builder.Services.AddSingleton(sp => new ConnectionFactory()
            {
                Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ"))
            });
            builder.Services.AddSingleton<IReportRepository, ReportRepository>();
            builder.Services.AddSingleton<RabbitMQPublisher>();
            builder.Services.AddSingleton<RabbitMQClientService>();
            builder.Services.AddSingleton<RabbitMQSubscriber>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
            
        }
    }
}
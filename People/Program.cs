using BackgroundServices;
using Microsoft.EntityFrameworkCore;
using People.Models;
using People.Services;
using RabbitMQ.Client;

namespace People
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



            builder.Services.AddSingleton(sp => new ConnectionFactory() { 
                Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ")),
                DispatchConsumersAsync = true });

            builder.Services.AddSingleton<RabbitMQClientService>();
            builder.Services.AddSingleton<RabbitMQPublisher>();

           

            builder.Services.AddHostedService<ReportProcessBackgroundService>();


            builder.Services.AddDbContext<AppDbContext>(
o => o.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));


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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using People.Models;

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

            //services.AddSingleton<RabbitMQClientService>();
            //services.AddSingleton<RabbitMQPublisher>();

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
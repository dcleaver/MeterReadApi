using MeterReadApi.Parsers;
using MeterReadApi.Persistence;
using MeterReadApi.Processors;
using MeterReadApi.Validators;
using Microsoft.EntityFrameworkCore;

namespace MeterReadApi
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

            builder.Services.AddScoped<IMeterReadingParser, CsvMeterReadingParser>();
            builder.Services.AddScoped<IMeterReadingRepository, MeterReadingRepository>();
            builder.Services.AddScoped<IMeterReadingProcessor, MeterReadingProcessor>();
            builder.Services.AddScoped<IMeterReadingValidator, MeterReadingValidator>();

            builder.Services.AddScoped<IAccountsSeeder, CsvAccountsSeeder>();

            builder.Services.AddDbContext<MeterReadingContext>(options => options.UseInMemoryDatabase("meterReadings"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
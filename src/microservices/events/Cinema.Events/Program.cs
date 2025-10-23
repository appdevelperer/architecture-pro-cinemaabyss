using System;
using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;



var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("1.0.0", new() { Title = "Cinema Events API", Version = "1.0.0" });
});

Console.WriteLine("Kafka parts is starting...");

// Kafka Producer
builder.Services
    .AddSingleton<ProducerConfig>(sp => new ProducerConfig
    {
        BootstrapServers = "kafka:9092" // или ваш Kafka-брокер
    });

builder.Services.AddSingleton<IKafkaEventPublisher, KafkaEventPublisher>();

// Kafka Consumer
builder.Services.AddHostedService<KafkaEventConsumer>();


var app = builder.Build();


app.UseDeveloperExceptionPage();


// Включите Swagger middleware
app.UseSwagger(c =>
{
    c.RouteTemplate = "openapi/{documentName}/openapi.json"; // стандартный путь
});

app.UseSwaggerUI(c => 
{
                    // set route prefix to openapi, e.g. http://localhost:8082/openapi/index.html
                    c.RoutePrefix = "openapi";
                    //TODO: Either use the SwaggerGen generated OpenAPI contract (generated from C# classes)
                    c.SwaggerEndpoint("/openapi/1.0.0/openapi.json", "CinemaAbyss API");
});

app.UseRouting();
app.MapControllers();

Console.WriteLine("Application is starting...");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"Application Name: {app.Environment.ApplicationName}");

app.Run();
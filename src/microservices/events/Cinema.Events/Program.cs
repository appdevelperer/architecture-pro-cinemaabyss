using System;
using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


var builder = WebApplication.CreateBuilder(args);

// Явно настройте логирование ПЕРВЫМ делом
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

Console.WriteLine("=== APPLICATION STARTING ===");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("1.0.0", new() { Title = "Cinema Events API", Version = "1.0.0" });
});

Console.WriteLine("=== KAFKA SERVICES REGISTRATION STARTING ===");

// Kafka Producer
try
{
    Console.WriteLine("Registering ProducerConfig...");
    builder.Services.AddSingleton<ProducerConfig>(new ProducerConfig
    {
        BootstrapServers = "kafka:9092"
    });
    Console.WriteLine("ProducerConfig registered successfully");
}
catch (Exception ex)
{
    Console.WriteLine($"Error registering ProducerConfig: {ex}");
}

// Kafka Publisher
try
{
    Console.WriteLine("Registering IKafkaEventPublisher...");
    builder.Services.AddSingleton<IKafkaEventPublisher, KafkaEventPublisher>();
    Console.WriteLine("IKafkaEventPublisher registered successfully");
}
catch (Exception ex)
{
    Console.WriteLine($"Error registering IKafkaEventPublisher: {ex}");
}

// Kafka Consumer
try
{
    Console.WriteLine("Registering KafkaEventConsumer...");
    // builder.Services.AddHostedService<KafkaEventConsumer>();
    builder.Services.AddSingleton<IKafkaEventReader, KafkaEventReader>();
    Console.WriteLine("KafkaEventConsumer registered successfully");
}
catch (Exception ex)
{
    Console.WriteLine($"Error registering KafkaEventConsumer: {ex}");
}

Console.WriteLine("=== ALL SERVICES REGISTERED ===");

var app = builder.Build();

// Проверка зарегистрированных сервисов
try
{
    Console.WriteLine("=== CHECKING REGISTERED SERVICES ===");
    using var scope = app.Services.CreateScope();
    var serviceProvider = scope.ServiceProvider;
    
    var kafkaPublisher = serviceProvider.GetService<IKafkaEventPublisher>();
    Console.WriteLine($"IKafkaEventPublisher: {kafkaPublisher != null}");
    
    var producerConfig = serviceProvider.GetService<ProducerConfig>();
    Console.WriteLine($"ProducerConfig: {producerConfig != null}");
    
    var kafkaConsumer = serviceProvider.GetService<KafkaEventConsumer>();
    Console.WriteLine($"KafkaEventConsumer: {kafkaConsumer != null}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error checking services: {ex}");
}

app.UseDeveloperExceptionPage();

app.UseSwagger(c =>
{
    c.RouteTemplate = "openapi/{documentName}/openapi.json";
});

app.UseSwaggerUI(c => 
{
    c.RoutePrefix = "openapi";
    c.SwaggerEndpoint("/openapi/1.0.0/openapi.json", "CinemaAbyss API");
});

app.UseRouting();
app.MapControllers();

Console.WriteLine("=== APPLICATION CONFIGURED ===");

app.Run();
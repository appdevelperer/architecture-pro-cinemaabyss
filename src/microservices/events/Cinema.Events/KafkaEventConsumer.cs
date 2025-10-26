using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class KafkaEventConsumer : BackgroundService
{

    private readonly ILogger<KafkaEventConsumer> _logger;
    private readonly string _bootstrapServers; 
    private readonly string _topic = "payment-events";
    private readonly string _groupId = "cinema-events-group";


    public KafkaEventConsumer(ILogger<KafkaEventConsumer> logger, IConfiguration configuration)
    {
        _logger = logger;
        _bootstrapServers = configuration["KAFKA_BROKERS"] ?? "kafka:9092"; // для Docker
        _logger.LogInformation("KafkaConsumer initialized with servers: {Servers}", _bootstrapServers);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Kafka consumer execution...");

        await Task.Delay(5000, stoppingToken);

        
        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = _groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        _logger.LogInformation("Consumer config created. Connecting to Kafka...");
        

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(_topic);

        _logger.LogInformation("Kafka consumer started, listening to topic: {Topic}", _topic);

        
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Используем асинхронное потребление с таймаутом
                    var result = consumer.Consume(TimeSpan.FromMilliseconds(1000));

                    if (result != null)
                    {
                        _logger.LogInformation("Received event: Key={Key}, Value={Value}",
                            result.Message.Key, result.Message.Value);

                        consumer.Commit(result);
                    }
                }
                catch (ConsumeException e)
                {
                    _logger.LogError(e, "Error consuming message");
                }
                
                // Даем возможность другим задачам выполняться
                await Task.Yield();
            }
        }
        finally
        {
            consumer.Close();
            _logger.LogInformation("Kafka consumer stopped");
        }
    }
}
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class KafkaEventConsumer : BackgroundService
{

     private readonly ILogger<KafkaEventConsumer> _logger;
    private readonly string _bootstrapServers = "kafka:9092"; 
    private readonly string _topic = "payment-events";
    private readonly string _groupId = "cinema-events-group";


    public KafkaEventConsumer(ILogger<KafkaEventConsumer> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = _groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(_topic);

        _logger.LogInformation("Kafka consumer started, listening to topic: {Topic}", _topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                _logger.LogInformation("Received event: Key={Key}, Value={Value}", 
                    result.Message.Key, result.Message.Value);
            }
            catch (ConsumeException e)
            {
                _logger.LogError(e, "Error consuming message");
            }
        }
    }
}
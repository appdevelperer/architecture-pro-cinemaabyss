using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

public interface IKafkaEventPublisher
{
    Task PublishAsync(string topic, string key, string value);
}

public class KafkaEventPublisher : IKafkaEventPublisher
{
    private readonly ProducerConfig _config;
    private readonly ILogger<KafkaEventPublisher> _logger;


 public KafkaEventPublisher(ProducerConfig config, ILogger<KafkaEventPublisher> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task PublishAsync(string topic, string key, string value)
    {
        using var producer = new ProducerBuilder<string, string>(_config).Build();
        await producer.ProduceAsync(topic, new Message<string, string> { Key = key, Value = value });
        _logger.LogInformation("Message published to Kafka. Topic: {Topic}, Key: {Key}", topic, key);
    }
}
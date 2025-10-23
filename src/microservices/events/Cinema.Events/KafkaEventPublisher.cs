using System.Threading.Tasks;
using Confluent.Kafka;

public interface IKafkaEventPublisher
{
    Task PublishAsync(string topic, string key, string value);
}

public class KafkaEventPublisher : IKafkaEventPublisher
{
    private readonly ProducerConfig _config;

    public KafkaEventPublisher(ProducerConfig config)
    {
        _config = config;
    }

    public async Task PublishAsync(string topic, string key, string value)
    {
        using var producer = new ProducerBuilder<string, string>(_config).Build();
        await producer.ProduceAsync(topic, new Message<string, string> { Key = key, Value = value });
    }
}
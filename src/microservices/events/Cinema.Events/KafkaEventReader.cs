using System;
using System.Collections.Generic;
using System.Threading;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

public interface IKafkaEventReader
{
    List<string> ReadPaymentEvents(int maxMessages = 10);
}

public class KafkaEventReader : IKafkaEventReader
{
    private readonly ILogger<KafkaEventReader> _logger;
    private readonly string _bootstrapServers = "kafka:9092";
    private readonly string _topic = "payment-events";
    private readonly string _groupId = "cinema-events-reader";

    public KafkaEventReader(ILogger<KafkaEventReader> logger)
    {
        _logger = logger;
    }

    public List<string> ReadPaymentEvents(int maxMessages = 10)
    {
        var messages = new List<string>();
        
        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = _groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            EnableAutoOffsetStore = false
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        
        try
        {
            consumer.Subscribe(_topic);
            _logger.LogInformation("Reading payment events from Kafka: {Value}", _topic);

            // Читаем сообщения с таймаутом
            for (int i = 0; i < maxMessages; i++)
            {
                try
                {
                    var result = consumer.Consume(TimeSpan.FromMilliseconds(1000));
                    
                    if (result != null)
                    {
                        _logger.LogInformation("Read payment event: {Value}", result.Message.Value);
                        messages.Add(result.Message.Value);
                        
                        // Подтверждаем обработку
                        consumer.Commit(result);
                        consumer.StoreOffset(result);
                    }
                    else
                    {
                        // Нет новых сообщений
                        break;
                    }
                }
                catch (ConsumeException e)
                {
                    _logger.LogError(e, "Error consuming message");
                    break;
                }
            }
        }
        finally
        {
            consumer.Close();
        }

        _logger.LogInformation("Read {Count} payment events from Kafka", messages.Count);
        return messages;
    }
}
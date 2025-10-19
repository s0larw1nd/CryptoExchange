namespace PriceFeed.Services;
using Confluent.Kafka;
using System;
using System.Threading.Tasks;
using Common;
using Models;

public class KafkaProducerService
{
    private readonly string topic;

    public KafkaProducerService(string tpc)
    {
        topic = tpc;
    }

    public async Task SendMessageAsync(string pair, int bid, int ask)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
            AllowAutoCreateTopics = true
        };

        using var producer = new ProducerBuilder<Null, string>(config).Build();

        var json = new PriceMessage
        {
            Prices = new[]
            {
                new PriceMessage.Price
                {
                    Pair = pair,
                    Bid = bid,
                    Ask = ask
                }
            }
        }.ToJson();

        try
        {
            var result = await producer.ProduceAsync(
                topic,
                new Message<Null, string> { Value = json }
            );
        }
        catch (ProduceException<string, string> ex)
        {
            //TODO
        }
    }
}

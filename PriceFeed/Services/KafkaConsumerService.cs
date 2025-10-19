using Confluent.Kafka;
using PriceFeed.BLL.Services;
using Common;
using Microsoft.Extensions.Options;
using PriceFeed.BLL.Models;
using PriceFeed.DAL;
using PriceFeed.Models;

namespace PriceFeed.Services;

public class KafkaConsumerService : BackgroundService
{
    private readonly KafkaSettings _settings;
    private readonly PricesService _pricesService;

    public KafkaConsumerService(IOptions<KafkaSettings> options, PricesService pricesService)
    {
        _settings = options.Value;
        _pricesService = pricesService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = _settings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Null, string>(config).Build();
        consumer.Subscribe(_settings.Topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var cr = consumer.Consume(stoppingToken);
                Console.WriteLine($"Received message: {cr.Message.Value}");
                await DoWorkAsync(cr.Message.Value, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            consumer.Close();
        }
    }
    
    private async Task DoWorkAsync(String message, CancellationToken stoppingToken)
    {
        var obj = message.FromJson<PriceMessage>();
        await _pricesService.BatchInsert(obj.Prices.Select(p => new PricesUnit
        {
            Pair = p.Pair,
            Bid = p.Bid,
            Ask = p.Ask
        }).ToArray(), stoppingToken);
    }
}
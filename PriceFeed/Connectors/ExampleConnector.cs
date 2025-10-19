using Confluent.Kafka;
using Microsoft.AspNetCore.Http.HttpResults;
using PriceFeed.Services;

namespace PriceFeed.Connectors;

public class ExampleConnector : BackgroundService
{
    private KafkaProducerService producer;
    private readonly string[] pairs = new[] { "USD/BTC" };
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        producer = new KafkaProducerService("prices.raw");
        
        Random random = new Random(); 
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            { 
                foreach (string pair in pairs) 
                {
                    await publish(pair, random.Next(90000,110000), random.Next(90000,110000));
                }
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                //TODO
            }
            catch (Exception ex)
            {
                //TODO
            }
        }
    }
    
    public async Task publish(String pair, int bid, int ask)
    {
        await producer.SendMessageAsync(pair, bid, ask);
    }

    public string[] supported_pairs()
    {
        return pairs;
    }
}
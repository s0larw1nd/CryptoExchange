using PriceFeed.Services;

namespace PriceFeed.Connectors;

public interface IConnector
{
    public Task run(CancellationToken stoppingToken);
    public Task publish(string pair, int bid, int ask);
    public string[] supported_pairs();
}
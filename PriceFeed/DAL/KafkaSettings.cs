using Microsoft.Extensions.Options;

namespace PriceFeed.DAL;

public class KafkaSettings
{
    public string Topic { get; set; }
    public string GroupId { get; set; }
}
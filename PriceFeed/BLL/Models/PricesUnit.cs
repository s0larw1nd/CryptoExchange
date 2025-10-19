namespace PriceFeed.BLL.Models;

public class PricesUnit
{
    public long Id { get; set; }
    public string Pair { get; set; }
    public long Bid { get; set; }
    public long Ask { get; set; }
    public DateTimeOffset Time { get; set; }
}
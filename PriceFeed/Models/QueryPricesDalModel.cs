namespace PriceFeed.Models;

public class QueryPricesDalModel
{
    public long[] Ids { get; set; }

    public string[] Pairs { get; set; }

    public int Limit { get; set; }

    public int Offset { get; set; }
}
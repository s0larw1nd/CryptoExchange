namespace PriceFeed.BLL.Models;

public class QueryPricesModel
{
    public long[] Ids { get; set; }

    public string[] Pairs { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }
}
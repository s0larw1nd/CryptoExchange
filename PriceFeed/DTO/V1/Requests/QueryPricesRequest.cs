namespace PriceFeed.DTO.V1.Requests;

public class QueryPricesRequest
{
    public string[] Pairs { get; set; }

    public int? Page { get; set; }

    public int? PageSize { get; set; }
}
using PriceFeed.BLL.Models;

namespace PriceFeed.DTO.V1.Responses;

public class QueryPricesResponce
{
    public PricesUnit[] Prices { get; set; }
}
namespace PriceFeed.Models;

public class PriceMessage
{
    public Price[] Prices { get; set; }

    public class Price
    {
        public string Pair { get; set; }
        public int Bid { get; set; }
        public int Ask { get; set; }
    }
}
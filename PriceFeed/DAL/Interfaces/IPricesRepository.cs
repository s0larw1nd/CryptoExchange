using PriceFeed.Models;

namespace PriceFeed.DAL.Interfaces;

public interface IPricesRepository
{
    Task<PricesDal[]> BulkInsert(PricesDal[] model, CancellationToken token);
    
    Task<PricesDal[]> Query(QueryPricesDalModel model, CancellationToken token);
}
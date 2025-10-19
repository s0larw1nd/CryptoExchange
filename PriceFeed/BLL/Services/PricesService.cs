using PriceFeed.BLL.Models;
using PriceFeed.DAL;
using PriceFeed.DAL.Interfaces;
using PriceFeed.Models;

namespace PriceFeed.BLL.Services;

public class PricesService(UnitOfWork unitOfWork, IPricesRepository pricesRepository)
{
    public async Task<PricesUnit[]> BatchInsert(PricesUnit[] pricesUnits, CancellationToken token)
    {
        var now = DateTimeOffset.UtcNow;
        await using var transaction = await unitOfWork.BeginTransactionAsync(token);
        
        try
        {
            PricesDal[] pricesDals = pricesUnits.Select(p => new PricesDal
            {
                Id=p.Id,
                Pair=p.Pair,
                Bid=p.Bid,
                Ask=p.Ask,
                Time=now
            }).ToArray();
            var prices = await pricesRepository.BulkInsert(pricesDals, token);
            
            await transaction.CommitAsync(token);
            return Map(prices);
        }
        catch (Exception e) 
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }

    public async Task<PricesUnit[]> GetPrices(QueryPricesModel model, CancellationToken token)
    {
        var prices = await pricesRepository.Query(new QueryPricesDalModel
        {
            Ids = model.Ids,
            Pairs = model.Pairs,
            Limit = model.PageSize,
            Offset = (model.Page - 1) * model.PageSize
        }, token);

        if (prices.Length is 0)
        {
            return [];
        }

        return Map(prices);
    }

    private PricesUnit[] Map(PricesDal[] prices)
    {
        return prices.Select(x => new PricesUnit
        {
            Id = x.Id,
            Pair = x.Pair,
            Bid = x.Bid,
            Ask = x.Ask,
            Time = x.Time
        }).ToArray();
    }
}
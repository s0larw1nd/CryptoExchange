using System.Text;
using Dapper;
using PriceFeed.DAL.Interfaces;
using PriceFeed.Models;

namespace PriceFeed.DAL.Repositories;

public class PricesRepository(UnitOfWork unitOfWork) : IPricesRepository
{
    public async Task<PricesDal[]> BulkInsert(PricesDal[] model, CancellationToken token)
    {
        var sql = @"
           INSERT INTO prices_raw
           (
                pair,
                bid,
                ask,
                time
           )
           
           SELECT
                pair,
                bid,
                ask,
                time
           FROM unnest(@Prices)
           
           RETURNING
                id,
                pair,
                bid,
                ask,
                time
        ";
        
        var conn = await unitOfWork.GetConnection(token);
        var res = await conn.QueryAsync<PricesDal>(new CommandDefinition(
            sql, new {Prices = model}, cancellationToken: token));
        
        return res.ToArray();
    }

    public async Task<PricesDal[]> Query(QueryPricesDalModel model, CancellationToken token)
    {
        var sql = new StringBuilder(@"
            select 
                id,
                pair,
                bid,
                ask,
                time
            from prices_raw
        ");
        
        var param = new DynamicParameters();
        
        var conditions = new List<string>();
        
        if (model.Ids?.Length > 0)
        {
            param.Add("Ids", model.Ids);
            conditions.Add("id = ANY(@Ids)");
        }
        
        if (model.Pairs?.Length > 0)
        {
            param.Add("Pairs", model.Pairs);
            conditions.Add("pair = ANY(@Pairs)");
        }
        
        if (conditions.Count > 0)
        {
            sql.Append(" where " + string.Join(" and ", conditions));
        }

        if (model.Limit > 0)
        {
            sql.Append(" limit @Limit");
            param.Add("Limit", model.Limit);
        }

        if (model.Offset > 0)
        {
            sql.Append(" offset @Offset");
            param.Add("Offset", model.Offset);
        }
        
        var conn = await unitOfWork.GetConnection(token);
        var res = await conn.QueryAsync<PricesDal>(new CommandDefinition(
            sql.ToString(), param, cancellationToken: token));
        
        return res.ToArray();
    }
}
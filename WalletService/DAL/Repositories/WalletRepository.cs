using WalletService.DAL.Interfaces;
using WalletService.Models;
using Dapper;

namespace WalletService.DAL.Repositories;

public class WalletRepository(UnitOfWork unitOfWork) : IWalletRepository
{
    public async Task<CurrencyDal[]> BulkInsert(CurrencyDal[] model, CancellationToken token)
    {
        var sql = @"
            INSERT INTO wallet
            (
                uid,
                currency,
                balance
            )

            SELECT
                uid,
                currency,
                balance
            FROM unnest(@User)

            RETURNING
                id,
                uid,
                currency,
                balance
        ";
        
        var conn = await unitOfWork.GetConnection(token);
        var res = await conn.QueryAsync<CurrencyDal>(new CommandDefinition(
            sql, new {User = model}, cancellationToken: token));
        
        return res.ToArray();
    }
    
    public async Task<CurrencyDal[]> BulkUpdate(CurrencyDal[] model, CancellationToken token)
    {
        var sql = @"
            UPDATE wallet w
            SET
                balance = u.balance
            FROM unnest(@User) AS u(
                uid,
                currency,
                balance
                )
            WHERE w.uid::bigint = u.uid::bigint
            AND w.currency = u.currency
            RETURNING
                w.id,
                w.uid,
                w.currency,
                w.balance;
        ";
        
        var conn = await unitOfWork.GetConnection(token);
        var res = await conn.QueryAsync<CurrencyDal>(new CommandDefinition(
            sql, new {User = model}, cancellationToken: token));
        
        return res.ToArray();
    }

    public async Task<CurrencyDal[]> Query(QueryCurrencyDalModel model, CancellationToken token)
    {
        var sql = @"
            SELECT 
                uid,
                currency,
                balance
            FROM wallet
            WHERE uid = @uid
        ";

        if (model.currency != null)
        {
            sql += " AND currency = @currency";
        }
        
        var conn = await unitOfWork.GetConnection(token);
        var res = await conn.QueryAsync<CurrencyDal>(
            new CommandDefinition(
                sql,
                new { model.uid, model.currency },
                cancellationToken: token
            ));
        
        return res.ToArray();
    }
}
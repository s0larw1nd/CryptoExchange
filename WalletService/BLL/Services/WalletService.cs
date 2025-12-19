using WalletService.BLL.Models;
using WalletService.DAL;
using WalletService.DAL.Interfaces;
using WalletService.Models;

namespace WalletService.BLL.Services;

public class WalletService(UnitOfWork unitOfWork, IWalletRepository walletRepository)
{
    public async Task<CurrencyUnit[]> BatchInsert(CurrencyUnit[] curUnits, CancellationToken token)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(token);

        try
        {
            CurrencyDal[] currDals = curUnits.Select(p => new CurrencyDal
            {
                id = p.id,
                uid = p.uid,
                currency = p.currency,
                balance = p.balance
            }).ToArray();
            var prices = await walletRepository.BulkInsert(currDals, token);
            
            await transaction.CommitAsync(token);
            return Map(prices);
        }
        catch (Exception e) 
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }

    public async Task<CurrencyUnit[]> BatchUpdate(CurrencyUnit[] curUnits, CancellationToken token)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(token);

        try
        {
            CurrencyDal[] currDals = curUnits.Select(p => new CurrencyDal
            {
                id = p.id,
                uid = p.uid,
                currency = p.currency,
                balance = p.balance
            }).ToArray();
            var prices = await walletRepository.BulkUpdate(currDals, token);
            
            await transaction.CommitAsync(token);
            return Map(prices);
        }
        catch (Exception e) 
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }

    public async Task<CurrencyUnit[]> Query(QueryCurrencyModel curUnit, CancellationToken token)
    {
        var curs = await walletRepository.Query(new QueryCurrencyDalModel
        {
            uid = curUnit.uid,
            currency = curUnit.currency,
        }, token);
        
        if (curs.Length is 0)
        {
            return [];
        }

        return Map(curs);
    }

    private CurrencyUnit[] Map(CurrencyDal[] currencyDals)
    {
        return currencyDals.Select(x => new CurrencyUnit
        {
            id = x.id,
            uid = x.uid,
            currency = x.currency,
            balance = x.balance
        }).ToArray();
    }
}
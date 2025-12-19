using WalletService.Models;

namespace WalletService.DAL.Interfaces;

public interface IWalletRepository
{
    Task<CurrencyDal[]> BulkInsert(CurrencyDal[] model, CancellationToken token);
    
    Task<CurrencyDal[]> BulkUpdate(CurrencyDal[] model, CancellationToken token);
    
    Task<CurrencyDal[]> Query(QueryCurrencyDalModel model, CancellationToken token);
}
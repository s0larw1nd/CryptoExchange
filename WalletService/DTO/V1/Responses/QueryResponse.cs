using WalletService.BLL.Models;

namespace WalletService.DTO.V1.Responses;

public class QueryResponse
{
    public CurrencyUnit[] Currencies { get; set; }
}
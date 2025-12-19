namespace WalletService.DTO.V1.Requests;

public class UpdateRequest
{
    public string token { get; set; }
    public string currency { get; set; }
    public int balance { get; set; }
}
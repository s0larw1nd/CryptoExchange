using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using WalletService.BLL.Models;
using WalletService.DTO.V1.Requests;
using WalletService.DTO.V1.Responses;

namespace WalletService.Controllers;

[Route("api/wallet")]
public class WalletController(BLL.Services.WalletService walletService): ControllerBase
{
    static HttpClient httpClient = new HttpClient();
    
    [HttpPost("insert")]
    public async Task<ActionResult<InsertResponse>> V1BatchCreate([FromBody] InsertRequest request,
        CancellationToken token)
    {
        string json = JsonSerializer.Serialize(new Dictionary<string,string>
        {
            ["Token"] = request.token
        });
        
        HttpResponseMessage response = await httpClient.PostAsync(
            "http://localhost:5034/api/auth/validate",
            new StringContent(json, Encoding.UTF8, "application/json"), token);
        
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync(token);
        using JsonDocument doc = JsonDocument.Parse(responseBody);
        long uid = long.Parse(doc.RootElement.GetProperty("userID").GetString());
        
        var res = await walletService.BatchInsert([
            new CurrencyUnit
            {
                uid = uid,
                currency = request.currency,
                balance = 0
            }
        ], token);
        
        return Ok(new InsertResponse
        {
        });
    }
    
    [HttpPost("update")]
    public async Task<ActionResult<UpdateResponse>> V1BatchUpdate([FromBody] UpdateRequest request,
        CancellationToken token)
    {
        string json = JsonSerializer.Serialize(new Dictionary<string,string>
        {
            ["Token"] = request.token
        });
        
        HttpResponseMessage response = await httpClient.PostAsync(
            "http://localhost:5034/api/auth/validate",
            new StringContent(json, Encoding.UTF8, "application/json"), token);
        
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync(token);
        using JsonDocument doc = JsonDocument.Parse(responseBody);
        long uid = long.Parse(doc.RootElement.GetProperty("userID").GetString());
        
        var res = await walletService.BatchUpdate([
            new CurrencyUnit
            {
                uid = uid,
                currency = request.currency,
                balance = request.balance
            }
        ], token);
        
        return Ok(new UpdateResponse
        {
        });
    }
    
    [HttpPost("query")]
    public async Task<ActionResult<QueryResponse>> QueryPrices([FromBody] QueryRequest request,
        CancellationToken token)
    {
        string json = JsonSerializer.Serialize(new Dictionary<string,string>
        {
            ["Token"] = request.uid
        });
        
        HttpResponseMessage response = await httpClient.PostAsync(
            "http://localhost:5034/api/auth/validate",
            new StringContent(json, Encoding.UTF8, "application/json"), token);
        
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync(token);
        using JsonDocument doc = JsonDocument.Parse(responseBody);
        long uid = long.Parse(doc.RootElement.GetProperty("userID").GetString());
        
        var res = await walletService.Query(new QueryCurrencyModel
        {
            uid = uid,
            currency = request.currency
        }, token);
        
        return Ok(new QueryResponse
        {
            Currencies = res
        });
    }
}
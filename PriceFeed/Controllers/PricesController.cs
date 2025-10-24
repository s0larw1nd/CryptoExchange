using System.Diagnostics;
using PriceFeed.DTO.V1.Requests;
using PriceFeed.DTO.V1.Responses;
using Microsoft.AspNetCore.Mvc;
using PriceFeed.BLL.Models;
using PriceFeed.BLL.Services;

namespace PriceFeed.Controllers;

[Route("api/prices")]
public class PricesController(PricesService pricesService): ControllerBase
{
    [HttpPost("query")]
    public async Task<ActionResult<QueryPricesResponce>> QueryPrices([FromBody] QueryPricesRequest request,
        CancellationToken token)
    {
        var res = await pricesService.GetPrices(new QueryPricesModel
        {
            Pairs = request.Pairs,
            Page = request.Page ?? 0,
            PageSize = request.PageSize ?? 0,
        }, token);
        
        return Ok(new QueryPricesResponce
        {
            Prices = res
        });
    }
}
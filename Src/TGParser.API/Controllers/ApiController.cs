using Leaf.xNet;
using Microsoft.AspNetCore.Mvc;
using TGParser.API.Attributes;
using TGParser.API.Services.Interfaces;
using TGParser.Configuration;
using TGParser.Core.DTO;
using TGParser.Core.Models.ApiRequestModels;
using ProxyType = TGParser.Core.Enums.ProxyType;

namespace TGParser.API.Controllers;

[ApiController]
[Produces("application/json")]
[Skip200Middleware]
[Route("[controller]/[action]")]
public class ApiController : ControllerBase
{
    /// <summary>
    /// Получить список объявлений.
    /// </summary>
    /// <param name="getAdsModel">Модель запроса объявления.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAds([FromServices] ISearchWallapopService searchWallapopService,
        [FromBody] GetAdsModel getAdsModel)
    {
        var unlimitToken = ConfigurationStorage.GetUnlimitToken();

        if (unlimitToken != getAdsModel.ApiKey)
            return Forbid();
        
        var preset = getAdsModel.Preset;
        var proxy = getAdsModel.Proxy;
        
        var result = await searchWallapopService
            .Search(string.Empty, new(default,
                proxy.IP, proxy.Port,
                proxy.User, proxy.Password,
                ProxyType.SOCKS5), new PresetDto(default, default, default, default, 
                preset.MinPrice, preset.MaxPrice,
                preset.MinDateRegisterSeller, preset.MaxDateRegisterSeller,
                preset.MaxNumberOfPublishBySeller, preset.MaxNumbersOfItemsSoldBySeller,
                preset.MaxNumberOfItemsBuysBySeller, preset.MaxViewsByOtherWorkers,
                preset.PeriodSearch));

        var items = result?.Data.Section.Payload.Items;
        
        return await Task.FromResult(Ok(items.Select(i => $"https://es.wallapop.com/item/{i.WebSlug}")));
    }
}
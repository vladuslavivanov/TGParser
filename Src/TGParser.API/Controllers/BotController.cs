using CryptoPay.Types;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Telegram.Bot.Types.Enums;
using TGParser.API.MassTransit.Requsted;
using TGParser.API.Services.Interfaces;
using TGParser.API.ActionFilters;
using TGParser.API.Controllers.CallbackQueries;
using TGParser.BLL.Interfaces;
using Update = Telegram.Bot.Types.Update;

namespace TGParser.API.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/[controller]/[action]")]
public class BotController(
    IBus bus, 
    IDialogService dialogService, 
    ITelegramUserService service, 
    CallbackQueryExecutor callbackQueryExecutor) : ControllerBase
{
    [HttpPost]
    [CheckTelegramToken]
    public async Task<IActionResult> Post(Update update)
    {
        if(update.Type == UpdateType.Message)
        {
            if (!await service.IsUserSubscribed(update.Message!)) return Ok();

            await service.AddUserIfNotExists(update.Message!);

            var userId = update.Message!.From!.Id;

            if (dialogService.CheckUserInDialog(userId))
                await bus.Publish(new RequestDialogCommand(update.Message));
            else
                await bus.Publish(new RequestMessageCommand(update));
        }
        else if (update.Type == UpdateType.CallbackQuery)
        {
            await callbackQueryExecutor.Execute(update);
        }

        return Ok();
    }

    [HttpPost]
    [CheckCryptoBotToken]
    public async Task<IActionResult> CryptoBot(IInvoiceManager invoiceManager)
    {
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();
        
        var update = JsonConvert.DeserializeObject<CryptoPay.Types.Update>(body, new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        });
        
        if (update == null)
            return BadRequest();
        
        if (update.Payload.Status != Statuses.paid)
            return Ok();

        var description = update.Payload.Description.Split('_');
            
        var quantityDays = int.Parse(description[0]);
        var price = double.Parse(description[1]);
        var userId = long.Parse(description[2]);

        var currency = string.IsNullOrEmpty(update.Payload.Fiat) ? 
            update.Payload.Asset : 
            update.Payload.Fiat;
        
        await invoiceManager.AddInvoice(new(
            currency, 
            price, 
            (DateTime)update.Payload.PaidAt!, 
            quantityDays,
            userId));
        
        return await Task.FromResult(Ok());
    }
}

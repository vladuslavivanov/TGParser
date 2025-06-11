using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TGParser.API.MassTransit.Requsted;
using TGParser.API.Services.Interfaces;
using TGParser.API.ActionFilters;
using TGParser.API.Controllers.CallbackQueries;

namespace TGParser.API.Controllers;

[ApiController]
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
    public async Task<IActionResult> CryptoBot(CryptoPay.Types.Update update)
    {
        return Ok();
    }
}

using MassTransit;
using Telegram.Bot;
using Telegram.Bot.Types;
using TGParser.API.Controllers.Commands.Interfaces;
using TGParser.API.MassTransit.Requsted;
using TGParser.API.Services.Interfaces;
using TGParser.BLL.Interfaces;
using TGParser.Core.Consts;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Commands.Implementations.Proxy;

public class AddProxyCommand(ITelegramBotClient client, 
    IProxyManager proxyManager, 
    IBus bus, IDialogService dialogService) 
    : BaseCommand, ICommand
{
    public string Name => CommandNames.ADD_PROXY;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var countProxy = await proxyManager
            .GetCountProxiesByUserId(UserId);

        if (countProxy >= BotConstants.MAX_PROXIES)
        {
            await client.SendMessage(ChatId,
                $"Доступно максимум {BotConstants.MAX_PROXIES} прокси❗");
            return;
        }

        dialogService.SetUserDialog(UserId, DialogType.AddProxy);

        await bus.Publish(new RequestDialogCommand(update.Message!));
    }
}

using MassTransit;
using Telegram.Bot;
using Telegram.Bot.Types;
using TGParser.API.Controllers.Commands.Interfaces;
using TGParser.API.MassTransit.Requsted;
using TGParser.API.Services.Interfaces;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Commands.Implementations.Parsing;

public class SearchWallapopCommand(
    IBus bus, 
    IDialogService dialogService, 
    IUserService userService,
    ITelegramBotClient client) : BaseCommand, ICommand
{
    public string Name => CommandNames.SEARCH_WALLAPOP;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var isSubscribed = await userService.IsUserSubscribed(UserId);

        if (!isSubscribed)
        {
            await client.SendMessage(ChatId, 
                "Для продолжения использования парсера необходимо оплатить подписку");

            dialogService.UserFinalDialog(UserId);

            return;
        }

        dialogService.SetUserDialog(UserId, DialogType.SearchWallapop);

        await bus.Publish(new RequestDialogCommand(update.Message!));
    }
}

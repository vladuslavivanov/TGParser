using MassTransit;
using Telegram.Bot.Types;
using TGParser.API.Controllers.Messages.ChatShared;
using TGParser.API.Controllers.Messages.ChatShared.Interfaces;
using TGParser.API.MassTransit.Requsted;
using TGParser.API.Services.Interfaces;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Messages.ChatShared.Implementations.Proxy;

public class TestProxyCommand(IBus bus, IDialogService dialogService) : BaseTelegramAction, ITextMessage
{
    public string Name => TextMessageNames.TEST_PROXY;

    public async Task Execute(Update update)
    {
        SetContext(update);

        dialogService.SetUserDialog(UserId, DialogType.TestProxy);

        await bus.Publish(new RequestDialogCommand(update.Message!));
    }
}

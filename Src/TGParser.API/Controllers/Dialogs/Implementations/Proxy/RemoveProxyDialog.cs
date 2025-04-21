using MassTransit;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.Commands;
using TGParser.API.Controllers.Dialogs.Interfaces;
using TGParser.API.Services.Interfaces;
using TGParser.BLL.Interfaces;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Dialogs.Implementations.Proxy;

public class RemoveProxyDialog(ITelegramBotClient client, 
    IBus bus, 
    IDialogService dialogService,
    IProxyManager proxyManager)
    : BaseDialog<BaseContext>(bus, dialogService, client), IDialog
{
    public DialogType DialogType => DialogType.RemoveProxy;

    public async Task Execute(Message message)
    {
        SetContext(new() { Message = message });

        _dialogContexts.TryGetValue(UserId, out var dialogContext);

        // Начало диалога.
        if (dialogContext == default)
        {
            _dialogContexts[UserId] = new();
            await SendMenu();
            return;
        }

        if (dialogContext.DialogState == DialogState.FirstStep)
        {
            if (await TryHandleUserLeaveAsync(nextCommandName: CommandNames.PROXIES))
                return;

            await TryRemoveProxy();

            await SendMenu();
        }
    }

    async Task SendMenu()
    {
        var allPresets = (await proxyManager
            .GetAllProxies(UserId)).OrderBy(o => o.ShowedId);

        var keyboard = new ReplyKeyboardMarkup(
        [
            allPresets.Select(p => new KeyboardButton(p.ShowedId.ToString())),
            [EditingNames.LEAVE]
        ])
        {
            ResizeKeyboard = true,
        };

        await client.SendMessage(
            chatId: ChatId,
            text: "Выберите прокси для удаления",
            replyMarkup: keyboard);
    }

    async Task TryRemoveProxy()
    {
        var parsed = int.TryParse(Message!.Text, out var selectedPreset);
        if (!parsed)
        {
            await client.SendMessage(ChatId, "Введите число!");
            return;
        }

        await proxyManager.RemoveProxy(UserId, selectedPreset);
    }
}

using MassTransit;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.Dialogs.Contexts;
using TGParser.API.Controllers.Dialogs.Interfaces;
using TGParser.API.Controllers.Messages.ChatShared;
using TGParser.API.Services.Interfaces;
using TGParser.BLL.Interfaces;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Dialogs.Implementations.Proxy;

public class AddProxyDialog(IBus bus, IDialogService dialogService, 
    ITelegramBotClient client, IProxyManager proxyManager) :
    BaseDialog<AddProxyContext>(bus, dialogService, client), IDialog
{
    public DialogType DialogType => DialogType.AddProxy;

    public async Task Execute(Message message)
    {
        SetContext(new() { Message = message });

        await TryHandleUserLeaveAsync(nextCommandName: TextMessageNames.PROXIES);

        _dialogContexts.TryGetValue(UserId, out var dialogContext);

        if (dialogContext == default)
            _dialogContexts[UserId] = dialogContext = new();


        if (dialogContext.DialogState == DialogState.FirstStep)
        {
            await SendMessage("Введите настройки прокси в формате 'IP:порт:логин:пароль'");
            dialogContext.DialogState++;
            return;
        }

        if (dialogContext.DialogState == DialogState.SecondStep)
        {
            var split = Message!.Text!.Split(':');

            if (split.Length != 4)
            {
                await SendMessage("Введите настройки прокси в формате 'IP:порт:логин:пароль'");
                return;
            }

            var isIp = IPAddress.TryParse(split[0], out var IP);
            var isPort = int.TryParse(split[1], out var port);
            var login = split[2];
            var password = split[3];

            if (!isIp)
            {
                await SendMessage("Введите IPv4, либо IPv6");
                return;
            }
            if (!isPort) 
            { 
                await SendMessage("Введите число");
                return;
            }

            if (port < 1 || port > 65535)
            {
                await SendMessage("Порт может принимать значение от 1 до 65535");
                return;
            }

            await proxyManager.AddProxy(new(UserId, IP.ToString(), port, login, password));

            Message.Text = EditingNames.LEAVE;
            await TryHandleUserLeaveAsync(nextCommandName: TextMessageNames.PROXIES);

            return;
        }
    }

    async Task SendMessage(string text)
    {
        var keyboard = new ReplyKeyboardMarkup(
         [
             [EditingNames.LEAVE]
         ])
        {
            ResizeKeyboard = true,
        };

        await client.SendMessage(ChatId, 
            text,
            replyMarkup:keyboard);
    }
}

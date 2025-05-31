using MassTransit;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.Dialogs.Contexts;
using TGParser.API.Controllers.Dialogs.Interfaces;
using TGParser.API.Controllers.Messages.ChatShared;
using TGParser.API.Services.Interfaces;
using TGParser.BLL.Implementations;
using TGParser.BLL.Interfaces;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Dialogs.Implementations.Proxy;

public class AddProxyDialog(IBus bus, IDialogService dialogService, 
    ITelegramBotClient client, IProxyManager proxyManager) :
    BaseDialog<AddProxyContext>(bus, dialogService, client), IDialog
{
    public DialogType DialogType => DialogType.AddProxy;

    //List<string> editableProperties = typeof(EditingNames.Proxy)
    //        .GetProperties()
    //        .Where(prop => Attribute.IsDefined(prop, typeof(EditableAttribute)))
    //        .Select(s => (string)s.GetRawConstantValue()!)
    //        .ToList();

    public async Task Execute(Message message)
    {
        SetContext(new() { Message = message });

        await TryHandleUserLeaveAsync(nextCommandName: TextMessageNames.PROXIES);

        _dialogContexts.TryGetValue(UserId, out var dialogContext);

        if (dialogContext == default)
            _dialogContexts[UserId] = dialogContext = new();

        if (dialogContext.DialogState == DialogState.FirstStep)
        {
            await SendMessage(EditingNames.Proxy.IP);
            dialogContext.LastRequest = EditingNames.Proxy.IP;
            dialogContext.DialogState++;
            return;
        }

        if (dialogContext.DialogState == DialogState.SecondStep)
        {
            var error = ValidateValue(Message!.Text!, dialogContext.LastRequest, out string result);

            if (error != default)
            {
                await client.SendMessage(ChatId, error);
                await SendMessage(EditingNames.Proxy.IP);
                return;
            }

            dialogContext.IP = result;

            await SendMessage(EditingNames.Proxy.PORT);
            dialogContext.LastRequest = EditingNames.Proxy.PORT;
            dialogContext.DialogState++;
            return;
        }

        if (dialogContext.DialogState == DialogState.ThirdStep)
        {
            var error = ValidateValue(Message!.Text!, dialogContext.LastRequest, out string result);

            if (error != default)
            {
                await client.SendMessage(ChatId, error);
                await SendMessage(EditingNames.Proxy.PORT);
                return;
            }

            dialogContext.Port = int.Parse(result);

            await SendMessage(EditingNames.Proxy.USER);
            dialogContext.LastRequest = EditingNames.Proxy.USER;
            dialogContext.DialogState++;
            return;
        }

        if (dialogContext.DialogState == DialogState.FourthStep)
        {
            dialogContext.UserName = Message!.Text!;

            await SendMessage(EditingNames.Proxy.PASSWORD);
            dialogContext.LastRequest = EditingNames.Proxy.PASSWORD;
            dialogContext.DialogState++;
            return;
        }

        if (dialogContext.DialogState == DialogState.FifthStep)
        {
            dialogContext.Password = Message!.Text!;

            await proxyManager.AddProxy(new(
                UserId, 
                dialogContext.IP, 
                dialogContext.Port, 
                dialogContext.UserName, 
                dialogContext.Password));

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

    string? ValidateValue(string value, string editingField, out string validateString)
    {
        string? errorMessage = null;

        switch (editingField)
        {
            case EditingNames.Proxy.IP:
                var isIp = IPAddress.TryParse(value, out var ip);
                if (!isIp)
                {
                    errorMessage = "Введите IPv4, либо IPv6";
                    break;
                }
                value = ip!.ToString();
                break;

            case EditingNames.Proxy.PORT:
                if (!int.TryParse(value, out var port))
                    errorMessage = "Введите число";
                if (port < 1 || port > 65535)
                    errorMessage = "Порт может принимать значение от 1 до 65535";
                break;

            case EditingNames.Proxy.PASSWORD:
            case EditingNames.Proxy.USER:
                break;

            default:
                errorMessage = "Непредвиденное свойство";
                break;
        }

        validateString = value;

        return errorMessage;
    }
}

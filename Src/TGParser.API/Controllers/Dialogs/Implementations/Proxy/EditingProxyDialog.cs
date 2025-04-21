using MassTransit;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.Commands;
using TGParser.API.Controllers.Dialogs.Contexts;
using TGParser.API.Controllers.Dialogs.Interfaces;
using TGParser.API.MassTransit.Requsted;
using TGParser.API.Services.Interfaces;
using TGParser.BLL.Interfaces;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Dialogs.Implementations.Proxy;

public class EditingProxyDialog(ITelegramBotClient client, 
    IProxyManager proxyManager, IBus bus, IDialogService dialogService) :
    BaseDialog<EditPropertyContext>(bus, dialogService, client), IDialog
{
    public DialogType DialogType => DialogType.EditingProxy;

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

        // На данном этапе пользователь уже выбрал сущность.
        if (dialogContext.DialogState == DialogState.FirstStep)
        {
            if (await TryHandleUserLeaveAsync(nextCommandName: CommandNames.PROXIES)) return;

            var isParsed = int.TryParse(message.Text, out var idEntity);

            if (isParsed == false)
            {
                await SendMenu();
                return;
            }

            var preset = await proxyManager.GetProxyByShowedIdAsync(UserId, idEntity);

            if (preset == default)
            {
                await SendMenu();
                return;
            }

            dialogContext.IdEntity = idEntity;

            //await client.SendMessage(ChatId, preset.ToString());

            await SendMessage(preset.ToString());

            await SendSelectedPropertyForEdit();
            dialogContext.DialogState = DialogState.SecondStep;
            return;
        }

        // На данном этапе уже выбрано свойство.
        if (dialogContext.DialogState == DialogState.SecondStep)
        {
            if (await TryHandleUserLeaveAsync(nextState: DialogState.FirstStep)) return;

            var isTextVerified = EditingNames.Proxy.IsPresetValue(message.Text!);

            if (!isTextVerified)
            {
                await SendSelectedPropertyForEdit();
                return;
            }

            dialogContext.ChoosedField = message.Text!;
            await SendEnterValueOrLeave();
            dialogContext.DialogState = DialogState.ThirdStep;
            return;
        }

        // На данном этапе уже указано новое значение.
        if (dialogContext.DialogState == DialogState.ThirdStep)
        {
            if (await TryHandleUserLeaveAsync(nextState: DialogState.SecondStep)) return;

            var errorMessage = ValidateValue(message.Text!, dialogContext.ChoosedField, out var validateString);

            if (errorMessage != default)
            {
                //await client.SendMessage(ChatId, errorMessage);
                await SendMessage(errorMessage);
                await SendEnterValueOrLeave();
                return;
            }

            PresetMapper.EditingNamePropertyInProxyTable.TryGetValue(dialogContext.ChoosedField, out var name);

            await proxyManager.UpdatePropertyInfoByPropertyName(UserId, dialogContext.IdEntity, name!, validateString);

            dialogContext.DialogState = DialogState.SecondStep;

            message.Text = "";

            await bus.Publish(new RequestDialogCommand(message));

            return;
        }
    }

    async Task SendMenu()
    {
        var allPresets = (await proxyManager.GetAllProxies(UserId)).OrderBy(o => o.ShowedId);

        var keyboard = new ReplyKeyboardMarkup(
        [
            allPresets.Select(p => new KeyboardButton(p.ShowedId.ToString())),
            [EditingNames.LEAVE]
        ])
        {
            ResizeKeyboard = true,
        };

        await SendMessage("Выберите прокси для редактирования", keyboard);

        //await client.SendMessage(
        //    chatId: ChatId,
        //    text: "Выберите прокси для редактирования",
        //    replyMarkup: keyboard);
    }

    async Task SendSelectedPropertyForEdit()
    {
        var keyboard = new ReplyKeyboardMarkup(
        [
            [EditingNames.Proxy.IP, EditingNames.Proxy.PORT],
            [EditingNames.Proxy.USER, EditingNames.Proxy.PASSWORD],
            [EditingNames.LEAVE],
        ])
        {
            ResizeKeyboard = true,
        };

        await SendMessage("Выберите свойство для редактирования", keyboard);
    }

    async Task SendEnterValueOrLeave()
    {
        var keyboard = new ReplyKeyboardMarkup(
        [
            [EditingNames.LEAVE],
        ])
        {
            ResizeKeyboard = true,
        };

        await SendMessage("Введите новое значение для параметра", keyboard);
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

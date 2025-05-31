using MassTransit;
using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.Dialogs.Contexts;
using TGParser.API.Controllers.Dialogs.Interfaces;
using TGParser.API.Controllers.Messages.ChatShared;
using TGParser.API.MassTransit.Requsted;
using TGParser.API.Services.Interfaces;
using TGParser.BLL.Interfaces;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Dialogs.Implementations.Preset;

public class EditingPresetDialog(ITelegramBotClient client,
    IPresetManager presetManager, IBus bus,
    IDialogService dialogService)
    : BaseDialog<EditPropertyContext>(bus, dialogService, client), IDialog
{
    public DialogType DialogType => DialogType.EditingPreset;

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
            if (await TryHandleUserLeaveAsync(nextCommandName: TextMessageNames.PRESETS)) return;

            var isParsed = int.TryParse(message.Text, out var idEntity);

            if (isParsed == false)
            {
                await SendMenu();
                return;
            }

            var preset = await presetManager.GetPresetByShowedIdAsync(UserId, idEntity);

            if (preset == default)
            {
                await SendMenu();
                return;
            }

            dialogContext.IdEntity = idEntity;

            await client.SendMessage(ChatId, preset.ToString());
            await SendSelectedPropertyForEdit();
            dialogContext.DialogState = DialogState.SecondStep;
            return;
        }

        // На данном этапе уже выбрано свойство.
        if (dialogContext.DialogState == DialogState.SecondStep)
        {
            if (await TryHandleUserLeaveAsync(nextState: DialogState.FirstStep)) return;

            var isTextVerified = EditingNames.Preset.IsPresetValue(message.Text!);

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

            var errorMessage = ValidateValue(message.Text!, dialogContext.ChoosedField, out string validateString);

            if (errorMessage != default)
            {
                await client.SendMessage(ChatId, errorMessage);
                await SendEnterValueOrLeave();
                return;
            }

            PresetMapper.EditingNamePropertyInPresetTable.TryGetValue(dialogContext.ChoosedField, out var name);

            await presetManager.UpdatePropertyInfoByPropertyName(UserId, dialogContext.IdEntity, name!, validateString);

            dialogContext.DialogState = DialogState.SecondStep;

            message.Text = "";

            await bus.Publish(new RequestDialogCommand(message));

            return;
        }

    }

    async Task SendMenu()
    {
        var allPresets = await presetManager.GetAllPresetsByUserIdAsync(UserId);

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
            text: "Выберите пресет для редактирования",
            replyMarkup: keyboard);
    }

    async Task SendSelectedPropertyForEdit()
    {
        var keyboard = new ReplyKeyboardMarkup(
        [
            [EditingNames.Preset.NAME],
            [EditingNames.Preset.MIN_PRICE, EditingNames.Preset.MAX_PRICE],
            [EditingNames.Preset.MIN_DATA_REGISTER_SELLER, EditingNames.Preset.MAX_DATA_REGISTER_SELLER],
            [EditingNames.Preset.MAX_VIEWS_BY_OTHER_WORKERS],
            [EditingNames.Preset.MAX_NUMBER_OF_ITEMS_SOLD_BY_SELLER],
            [EditingNames.Preset.MAX_NUNMBER_OF_ITEMS_BUYS_BY_SELLER],
            [EditingNames.Preset.MAX_NUMBER_OF_PUBLISH_BY_SELLER],
            [EditingNames.Preset.SEARCH_PERIOD_IN_DAYS],
            [EditingNames.LEAVE],
        ])
        {
            ResizeKeyboard = true,
        };

        var message = await client.SendMessage(
                chatId: ChatId,
                text: "Выберите свойство для редактирования",
                replyMarkup: keyboard
                );
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

        var message = await client.SendMessage(
                chatId: ChatId,
                text: "Введите новое значение для параметра",
                replyMarkup: keyboard
                );
    }

    string? ValidateValue(string value, string editingField, out string validateString)
    {
        string? errorMessage = null;

        switch (editingField)
        {
            case EditingNames.Preset.NAME:
                if (value.Length > 25)
                    errorMessage = "Введите название пресета менее 25 символов";
                break;

            case EditingNames.Preset.MAX_NUNMBER_OF_ITEMS_BUYS_BY_SELLER:
            case EditingNames.Preset.MAX_NUMBER_OF_ITEMS_SOLD_BY_SELLER:
            case EditingNames.Preset.MAX_VIEWS_BY_OTHER_WORKERS:
            case EditingNames.Preset.MAX_PRICE:
            case EditingNames.Preset.MIN_PRICE:
            case EditingNames.Preset.MAX_NUMBER_OF_PUBLISH_BY_SELLER:
                if (!int.TryParse(value, out var _))
                    errorMessage = "Введите число";
                break;
            case EditingNames.Preset.MAX_DATA_REGISTER_SELLER:
            case EditingNames.Preset.MIN_DATA_REGISTER_SELLER:
                if (!DateTime.TryParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    errorMessage = "Введите дату в формате дд.ММ.гггг";
                break;
            case EditingNames.Preset.SEARCH_PERIOD_IN_DAYS:
                if (!Enum.IsDefined(typeof(PeriodSearch), value.ToUpper()))
                {
                    var values = Enum.GetValues(typeof(PeriodSearch));
                    string result = string.Join(", ", values.Cast<PeriodSearch>().Select(v => v.ToString()));
                    
                    errorMessage = $"Период поиска объявлений может быть одним из следующих значений: {result}";
                    break;
                }

                PeriodSearch periodSearch = (PeriodSearch)Enum.Parse(typeof(PeriodSearch), value.ToUpper());

                validateString = ((int)periodSearch).ToString();

                return string.Empty;
            
            default:
                errorMessage = "Непредвиденное свойство";
                break;
        }

        validateString = value;

        return errorMessage;
    }
}

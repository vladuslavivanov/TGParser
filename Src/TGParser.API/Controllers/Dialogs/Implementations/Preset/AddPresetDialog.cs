using MassTransit;
using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.Commands;
using TGParser.API.Controllers.Dialogs.Contexts;
using TGParser.API.Controllers.Dialogs.Interfaces;
using TGParser.API.Services.Interfaces;
using TGParser.BLL.Interfaces;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Dialogs.Implementations.Preset;

public class AddPresetDialog(IBus bus, IDialogService dialogService, 
    ITelegramBotClient client, IPresetManager presetManager) : 
    BaseDialog<AddPresetContext>(bus, dialogService, client), IDialog
{
    public DialogType DialogType => DialogType.AddPreset;

    public async Task Execute(Message message)
    {
        SetContext(new() { Message = message });

        await TryHandleUserLeaveAsync(nextCommandName: CommandNames.PRESETS);

        _dialogContexts.TryGetValue(UserId, out var dialogContext);

        if (dialogContext == default)
            _dialogContexts[UserId] = dialogContext = new();

        if (dialogContext.DialogState == DialogState.FirstStep)
        {
            await SendMessage(EditingNames.Preset.NAME);
            dialogContext.LastRequest = EditingNames.Preset.NAME;
            dialogContext.DialogState++;
            return;
        }

        if (dialogContext.DialogState == DialogState.SecondStep)
        {
            dialogContext.PresetName = Message!.Text;

            await SendMessage(EditingNames.Preset.MAX_PRICE);
            dialogContext.LastRequest = EditingNames.Preset.MAX_PRICE;
            dialogContext.DialogState++;
            return;
        }

        if (dialogContext.DialogState == DialogState.ThirdStep)
        {
            var error = ValidateValue(Message!.Text!, dialogContext.LastRequest, out string result);

            if (error != default)
            {
                await client.SendMessage(ChatId, error);
                await SendMessage(EditingNames.Preset.MAX_PRICE);
                return;
            }

            dialogContext.MaxPrice = int.Parse(result);

            await SendMessage(EditingNames.Preset.MIN_PRICE);
            dialogContext.LastRequest = EditingNames.Preset.MIN_PRICE;
            dialogContext.DialogState++;
            return;
        }

        if (dialogContext.DialogState == DialogState.FourthStep)
        {
            var error = ValidateValue(Message!.Text!, dialogContext.LastRequest, out string result);

            if (error != default)
            {
                await client.SendMessage(ChatId, error);
                await SendMessage(EditingNames.Preset.MIN_PRICE);
                return;
            }

            dialogContext.MinPrice = int.Parse(result);

            await SendMessage(EditingNames.Preset.MAX_DATA_REGISTER_SELLER);
            dialogContext.LastRequest = EditingNames.Preset.MAX_DATA_REGISTER_SELLER;
            dialogContext.DialogState++;
            return;
        }

        if (dialogContext.DialogState == DialogState.FifthStep)
        {
            var error = ValidateValue(Message!.Text!, dialogContext.LastRequest, out string result);

            if (error != default)
            {
                await client.SendMessage(ChatId, error);
                await SendMessage(EditingNames.Preset.MAX_DATA_REGISTER_SELLER);
                return;
            }

            dialogContext.MaxDateRegisterSaller = DateTime.Parse(result);

            await SendMessage(EditingNames.Preset.MIN_DATA_REGISTER_SELLER);
            dialogContext.LastRequest = EditingNames.Preset.MIN_DATA_REGISTER_SELLER;
            dialogContext.DialogState++;
            return;
        }

        if (dialogContext.DialogState == DialogState.SixthStep)
        {
            var error = ValidateValue(Message!.Text!, dialogContext.LastRequest, out string result);

            if (error != default)
            {
                await client.SendMessage(ChatId, error);
                await SendMessage(EditingNames.Preset.MIN_DATA_REGISTER_SELLER);
                return;
            }

            dialogContext.MinDateRegisterSeller = DateTime.Parse(result);

            await SendMessage(EditingNames.Preset.MAX_VIEWS_BY_OTHER_WORKERS);
            dialogContext.LastRequest = EditingNames.Preset.MAX_VIEWS_BY_OTHER_WORKERS;
            dialogContext.DialogState++;
            return;
        }

        if (dialogContext.DialogState == DialogState.SeventhStep)
        {
            var error = ValidateValue(Message!.Text!, dialogContext.LastRequest, out string result);

            if (error != default)
            {
                await client.SendMessage(ChatId, error);
                await SendMessage(EditingNames.Preset.MAX_VIEWS_BY_OTHER_WORKERS);
                return;
            }

            dialogContext.MaxViewsByOthersWorkers = int.Parse(result);

            await SendMessage(EditingNames.Preset.MAX_NUMBER_OF_ITEMS_SOLD_BY_SELLER);
            dialogContext.LastRequest = EditingNames.Preset.MAX_NUMBER_OF_ITEMS_SOLD_BY_SELLER;
            dialogContext.DialogState++;
            return;
        }

        if (dialogContext.DialogState == DialogState.EighthStep)
        {
            var error = ValidateValue(Message!.Text!, dialogContext.LastRequest, out string result);

            if (error != default)
            {
                await client.SendMessage(ChatId, error);
                await SendMessage(EditingNames.Preset.MAX_NUMBER_OF_ITEMS_SOLD_BY_SELLER);
                return;
            }

            dialogContext.MaxNumberOfItemsSoldBySeller = int.Parse(result);

            await SendMessage(EditingNames.Preset.MAX_NUNMBER_OF_ITEMS_BUYS_BY_SELLER);
            dialogContext.LastRequest = EditingNames.Preset.MAX_NUNMBER_OF_ITEMS_BUYS_BY_SELLER;
            dialogContext.DialogState++;
            return;
        }

        if (dialogContext.DialogState == DialogState.NinthStep)
        {
            var error = ValidateValue(Message!.Text!, dialogContext.LastRequest, out string result);

            if (error != default)
            {
                await client.SendMessage(ChatId, error);
                await SendMessage(EditingNames.Preset.MAX_NUNMBER_OF_ITEMS_BUYS_BY_SELLER);
                return;
            }

            dialogContext.MaxNumberOfItemsBuysBySeller = int.Parse(result);

            await SendMessage(EditingNames.Preset.MAX_NUMBER_OF_PUBLISH_BY_SELLER);
            dialogContext.LastRequest = EditingNames.Preset.MAX_NUMBER_OF_PUBLISH_BY_SELLER;
            dialogContext.DialogState++;
            return;
        }

        if (dialogContext.DialogState == DialogState.TenthStep)
        {
            var error = ValidateValue(Message!.Text!, dialogContext.LastRequest, out string result);

            if (error != default)
            {
                await client.SendMessage(ChatId, error);
                await SendMessage(EditingNames.Preset.MAX_NUMBER_OF_PUBLISH_BY_SELLER);
                return;
            }

            dialogContext.MaxNumberOfPublishBySeller = int.Parse(result);

            await SendMessage(EditingNames.Preset.SEARCH_PERIOD_IN_DAYS);
            dialogContext.LastRequest = EditingNames.Preset.SEARCH_PERIOD_IN_DAYS;
            dialogContext.DialogState++;
            return;

        }

        if (dialogContext.DialogState == DialogState.EleventhStep)
        {
            var error = ValidateValue(Message!.Text!, dialogContext.LastRequest, out string result);

            if (error != default)
            {
                await client.SendMessage(ChatId, error);
                await SendMessage(EditingNames.Preset.SEARCH_PERIOD_IN_DAYS);
                return;
            }

            dialogContext.PeriodSearch = Enum.Parse<PeriodSearch>(result);

            await presetManager.AddPreset(new(
                UserId,
                dialogContext.PresetName,
                dialogContext.MinPrice,
                dialogContext.MaxPrice,
                dialogContext.MinDateRegisterSeller,
                dialogContext.MaxDateRegisterSaller,

                dialogContext.MaxNumberOfPublishBySeller,
                dialogContext.MaxNumberOfItemsSoldBySeller,
                dialogContext.MaxNumberOfItemsBuysBySeller,
                
                dialogContext.MaxViewsByOthersWorkers,
                dialogContext.PeriodSearch));

            Message.Text = EditingNames.LEAVE;
            await TryHandleUserLeaveAsync(nextCommandName: CommandNames.PRESETS);

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
            replyMarkup: keyboard);
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

                return errorMessage;

            default:
                errorMessage = "Непредвиденное свойство";
                break;
        }

        validateString = value;

        return errorMessage;
    }
}
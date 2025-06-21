using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.CallbackQueries.Interfaces;
using TGParser.API.Controllers.Helpers;
using TGParser.BLL.Interfaces;
using TGParser.Core.Enums;
using TGParser.Core.Enums.Presets;

namespace TGParser.API.Controllers.CallbackQueries.Implementations.PresetImpl;

public class SetRegistrationPresetCallbackQuery(ITelegramBotClient client, IPresetManager presetManager) : BaseTelegramAction, ICallbackQuery
{
    public string Name => CallbackQueryNames.SET_REGISTRATION_PRESET;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var splitData = CallbackQueryData!.Split('_').ToList();

        var presetId = int.Parse(splitData[1]);
        var stepSet = Enum.Parse<SetRegistrationPresetStep>(splitData.ElementAtOrDefault(2) ?? SetRegistrationPresetStep.ShowTypeRegistration.ToString());
        Enum.TryParse<RegistrationDataSellerType>(splitData.ElementAtOrDefault(3), out var selectedRegistrationType);
        int.TryParse(splitData.ElementAtOrDefault(4), out var year);
        int.TryParse(splitData.ElementAtOrDefault(5), out var month);

        switch (stepSet)
        {
            case SetRegistrationPresetStep.ShowTypeRegistration:
                await ShowTypeRegistrationStep(presetId);
                break;
            case SetRegistrationPresetStep.SelectYearRegistration:
                await ShowVariableRegistrationYears(presetId, selectedRegistrationType);
                break;
            case SetRegistrationPresetStep.SelectMonthRegistration:
                await ShowVariableRegistrationMonth(presetId, selectedRegistrationType, year);
                break;
            case SetRegistrationPresetStep.SetValue:
                await presetManager.SetSellerRegistration(UserId, presetId, selectedRegistrationType, new DateTime(year, month, 1));
                await UpdateMessageHelper.UpdateUserPreset(client, presetManager, UserId, presetId, (int)BotMessageId!, Message!.ReplyMarkup);
                await client.AnswerCallbackQuery(CallbackQueryId!, "✅ Готово");
                break;
        }

        throw new NotImplementedException();
    }

    async Task ShowTypeRegistrationStep(int presetId)
    {
        var propertyToEdit = new InlineKeyboardMarkup(
        [
            [
                InlineKeyboardButton.WithCallbackData("📉 Дата мин. рег-ии", $"{CallbackQueryNames.SET_REGISTRATION_PRESET}_{presetId}_{(int)SetRegistrationPresetStep.SelectYearRegistration}_{(int)RegistrationDataSellerType.Min}"),
                InlineKeyboardButton.WithCallbackData("📈 Дата макс. рег-ии", $"{CallbackQueryNames.SET_REGISTRATION_PRESET}_{presetId}_{(int)SetRegistrationPresetStep.SelectYearRegistration}_{(int)RegistrationDataSellerType.Max}")
            ],
            [
                InlineKeyboardButton.WithCallbackData("◀️ Назад", $"{CallbackQueryNames.EDIT_PRESET}_{presetId}")
            ]
        ]);

        await client.EditMessageReplyMarkup(ChatId, (int)BotMessageId!, propertyToEdit);
    }

    async Task ShowVariableRegistrationYears(int presetId, RegistrationDataSellerType regType)
    {
        var years = Enumerable.Range(2014, DateTime.Now.Year - 2014 + 1);

        var grouped = years
            .Select((value, index) => new { value, index })
            .GroupBy(x => x.index / 3)
            .Select(g => g.Select(x => x.value).ToArray())
            .ToList();

        InlineKeyboardMarkup keyboardMarkup = new();

        foreach (var item in grouped)
        {
            var buttons = item.Select(y => InlineKeyboardButton.WithCallbackData(y.ToString(),
                $"{CallbackQueryNames.SET_REGISTRATION_PRESET}_{presetId}_{(int)SetRegistrationPresetStep.SelectMonthRegistration}_{regType}_{y}"))
                .ToArray();

            keyboardMarkup.AddNewRow(buttons);
        }

        keyboardMarkup.AddNewRow(InlineKeyboardButton.WithCallbackData("◀️ Назад", $"{CallbackQueryNames.SET_REGISTRATION_PRESET}_{presetId}_{(int)SetRegistrationPresetStep.ShowTypeRegistration}"));

        await client.EditMessageReplyMarkup(ChatId, (int)BotMessageId!, keyboardMarkup);
    }

    async Task ShowVariableRegistrationMonth(int presetId, RegistrationDataSellerType regType, int year)
    {
        var culture = new CultureInfo("ru-RU");

        var numMonth = 1;

        var months = culture.DateTimeFormat.MonthNames
            .Where(m => !string.IsNullOrEmpty(m))
            .Select(m => new KeyValuePair<int, string>(numMonth++, m))
            .ToArray();

        var grouped = months
            .Select((value, index) => new { value, index })
            .GroupBy(x => x.index / 3)
            .Select(g => g.Select(x => x.value).ToArray())
            .ToList();

        InlineKeyboardMarkup keyboardMarkup = new();

        foreach (var item in grouped)
        {
            var buttons = item.Select(m => InlineKeyboardButton.WithCallbackData(m.Value.ToString(),
                $"{CallbackQueryNames.SET_REGISTRATION_PRESET}_{presetId}_{(int)SetRegistrationPresetStep.SetValue}_{regType}_{year}_{m.Key}"))
                .ToArray();

            keyboardMarkup.AddNewRow(buttons);
        }

        keyboardMarkup.AddNewRow(InlineKeyboardButton.WithCallbackData("◀️ Назад", $"{CallbackQueryNames.SET_REGISTRATION_PRESET}_{presetId}_{(int)SetRegistrationPresetStep.ShowTypeRegistration}"));

        await client.EditMessageReplyMarkup(ChatId, (int)BotMessageId!, keyboardMarkup);
    }
}

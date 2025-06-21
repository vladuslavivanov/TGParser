using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.CallbackQueries.Interfaces;
using TGParser.API.Controllers.Helpers;
using TGParser.BLL.Interfaces;
using TGParser.Core.Enums;
using TGParser.Core.Enums.Presets;

namespace TGParser.API.Controllers.CallbackQueries.Implementations.PresetImpl;

public class SetLimitationsPresetCallbackQuery(ITelegramBotClient client, IPresetManager presetManager) : BaseTelegramAction, ICallbackQuery
{
    public string Name => CallbackQueryNames.SET_LIMITATIONS_PRESET;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var splitData = CallbackQueryData!.Split('_').ToList();

        var presetId = int.Parse(splitData[1]);

        Enum.TryParse(splitData.ElementAtOrDefault(2) ?? SetLimitationsPresetStep.SHOW_LIMINATIONS.ToString(), out SetLimitationsPresetStep step);

        int.TryParse(splitData.ElementAtOrDefault(3), out var selectedLimitation);
        
        int.TryParse(splitData.ElementAtOrDefault(4), out var interval);
        
        int.TryParse(splitData.ElementAtOrDefault(5), out var result);

        switch (step)
        {
            case SetLimitationsPresetStep.SHOW_LIMINATIONS:
                await ShowLimitations(presetId);
                break;
            case SetLimitationsPresetStep.SELECT_INTERVAL:
                await ShowIntervals(presetId, selectedLimitation);
                break;
            case SetLimitationsPresetStep.SELECT_NUM:
                await ShowNum(presetId, selectedLimitation, interval);
                break;
            case SetLimitationsPresetStep.SET:
                await presetManager.SetLimitation(UserId, presetId, Enum.Parse<LimitationType>(selectedLimitation.ToString()), result);
                await UpdateMessageHelper.UpdateUserPreset(client, presetManager, UserId, presetId, (int)BotMessageId!, Message!.ReplyMarkup);
                await client.AnswerCallbackQuery(CallbackQueryId!, "✅ Готово");
                break;
        }
    }

    async Task ShowLimitations(int presetId)
    {
        var propertyToEdit = new InlineKeyboardMarkup(
        [
            [InlineKeyboardButton.WithCallbackData("📈 Кол-во просмотров товара другими воркерами", $"{CallbackQueryNames.SET_LIMITATIONS_PRESET}_{presetId}_{(int)SetLimitationsPresetStep.SELECT_INTERVAL}_{(int)LimitationType.QUANTITY_ADV_OTHER_VIEW}"),],
            [InlineKeyboardButton.WithCallbackData("📈 Кол-во открытых объявлений у продавца", $"{CallbackQueryNames.SET_LIMITATIONS_PRESET}_{presetId}_{(int)SetLimitationsPresetStep.SELECT_INTERVAL}_{(int)LimitationType.QUANTITY_OPEN_ADV}")],
            [InlineKeyboardButton.WithCallbackData("📈 Кол-во проданных товаров продавцом", $"{CallbackQueryNames.SET_LIMITATIONS_PRESET}_{presetId}_{(int)SetLimitationsPresetStep.SELECT_INTERVAL}_{(int)LimitationType.QUANTITY_SELLED_ADV}")],
            [InlineKeyboardButton.WithCallbackData("📈 Кол-во купленных товаров продавцом", $"{CallbackQueryNames.SET_LIMITATIONS_PRESET}_{presetId}_{(int)SetLimitationsPresetStep.SELECT_INTERVAL}_{(int)LimitationType.QUANTITY_BOUGHT_ADV}")],
            
            [InlineKeyboardButton.WithCallbackData("◀️ Назад", $"{CallbackQueryNames.EDIT_PRESET}_{presetId}")]
        ]);

        await client.EditMessageReplyMarkup(ChatId, (int)BotMessageId!, propertyToEdit);
    }

    async Task ShowIntervals(int presetId, int selectedLimitation)
    {
        var range = Enumerable.Range(0, 10);

        var grouped = range
                    .Select((value, index) => new { value, index })
                    .GroupBy(x => x.index / 3)
                    .Select(g => g.Select(x => x.value * 10).ToArray())
                    .ToList();

        InlineKeyboardMarkup keyboardMarkup = new();

        foreach (var item in grouped)
        {
            var buttons = item.Select(i => InlineKeyboardButton.WithCallbackData($"{i} - {i + 9}",
                $"{CallbackQueryNames.SET_LIMITATIONS_PRESET}_{presetId}_{(int)SetLimitationsPresetStep.SELECT_NUM}_{selectedLimitation}_{i}"))
                .ToArray();

            keyboardMarkup.AddNewRow(buttons);
        }

        keyboardMarkup.AddNewRow(InlineKeyboardButton.WithCallbackData("◀️ Назад", $"{CallbackQueryNames.SET_LIMITATIONS_PRESET}_{presetId}"));

        await client.EditMessageReplyMarkup(ChatId, (int)BotMessageId!, keyboardMarkup);
    }

    async Task ShowNum(int presetId, int selectedLimitation, int interval)
    {
        var range = Enumerable.Range(interval, 10);

        var grouped = range
                    .Select((value, index) => new { value, index })
                    .GroupBy(x => x.index / 3)
                    .Select(g => g.Select(x => x.value).ToArray())
                    .ToList();

        InlineKeyboardMarkup keyboardMarkup = new();

        foreach (var item in grouped)
        {
            var buttons = item.Select(num => InlineKeyboardButton.WithCallbackData($"{num}",
                $"{CallbackQueryNames.SET_LIMITATIONS_PRESET}_{presetId}_{(int)SetLimitationsPresetStep.SET}_{selectedLimitation}_{interval}_{num}"))
                .ToArray();

            keyboardMarkup.AddNewRow(buttons);
        }

        keyboardMarkup.AddNewRow(InlineKeyboardButton.WithCallbackData("◀️ Назад", $"{CallbackQueryNames.SET_LIMITATIONS_PRESET}_{presetId}"));

        await client.EditMessageReplyMarkup(ChatId, (int)BotMessageId!, keyboardMarkup);
    }
}

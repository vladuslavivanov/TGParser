using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.CallbackQueries;

namespace TGParser.API.Controllers.Messages.Helpers;

public static class ConfigureReplyMarkupHelper
{
    public static InlineKeyboardMarkup ConfigureMarkupForPresets(bool isSelected, int presetId, int? nextPresetId, int? previouslyPresetId)
    {
        var answer = new InlineKeyboardMarkup();

        if (previouslyPresetId != null)
        {
            answer.AddButton(InlineKeyboardButton.WithCallbackData("◀️", $"{CallbackQueryNames.SHOW_PRESET}_{previouslyPresetId}"));
        }

        if (nextPresetId != null)
        {
            answer.AddButton(InlineKeyboardButton.WithCallbackData("▶️", $"{CallbackQueryNames.SHOW_PRESET}_{nextPresetId}"));
        }

        answer.AddNewRow(
            [
                InlineKeyboardButton.WithCallbackData("📝 Изменить", $"{CallbackQueryNames.EDIT_PRESET}_{presetId}"),
                InlineKeyboardButton.WithCallbackData("➕ Добавить новый пресет", CallbackQueryNames.ADD_PRESET)
            ]);


        if (!isSelected)
        {
            answer.AddNewRow(InlineKeyboardButton.WithCallbackData("✅ Выбрать по умолчанию", $"{CallbackQueryNames.SELECT_DEFAULT_PRESET}_{presetId}"));
        }

        return answer;
    }
}

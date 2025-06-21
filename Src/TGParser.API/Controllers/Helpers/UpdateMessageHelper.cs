using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.BLL.Interfaces;

namespace TGParser.API.Controllers.Helpers;

public static class UpdateMessageHelper
{
    public static async Task UpdateUserPreset(ITelegramBotClient client, IPresetManager presetManager,
        long userId, int presetShowId, long botMessageId, InlineKeyboardMarkup? markup = null)
    {
        var preset = await presetManager.GetPresetByShowedIdAsync(userId, presetShowId);

        if (preset == default)
        {
            await client.EditMessageText(userId, (int)botMessageId,
                "Пресет не найден ❌", replyMarkup: InlineKeyboardMarkup.Empty());
            return;
        }

        await client.EditMessageText(userId, (int)botMessageId!, preset.ToString(), replyMarkup: markup);        
    }
}

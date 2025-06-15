using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.CallbackQueries.Interfaces;
using TGParser.API.Controllers.Messages.Helpers;
using TGParser.BLL.Interfaces;

namespace TGParser.API.Controllers.CallbackQueries.Implementations;

public class ShowPresetCallbackQuery(ITelegramBotClient client,
    IPresetManager presetManager) : BaseTelegramAction, ICallbackQuery
{
    public string Name => CallbackQueryNames.SHOW_PRESET;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var presetToShow = int.Parse(CallbackQueryData!.Split('_')[1]);

        var presets = await presetManager.GetAllPresetsByUserIdAsync(UserId);

        var preset = presets.FirstOrDefault(p => p.ShowedId == presetToShow);

        if (preset == default)
        {
            await client.EditMessageText(UserId, (int)BotMessageId!, 
                "Пресеты не найдены ❌", replyMarkup: InlineKeyboardMarkup.Empty());
            return;
        }

        await client.EditMessageText(UserId, (int)BotMessageId!, preset.ToString(), 
                replyMarkup: ConfigureReplyMarkupHelper.ConfigureMarkupForPresets(
                    preset.IsSelected, preset.ShowedId, presets.FirstOrDefault(p => p.ShowedId == preset.ShowedId + 1)?.ShowedId, 
                    presets.FirstOrDefault(p => p.ShowedId == preset.ShowedId - 1)?.ShowedId));
    }
}

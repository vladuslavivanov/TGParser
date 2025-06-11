using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.CallbackQueries;
using TGParser.API.Controllers.Messages.ChatShared.Interfaces;
using TGParser.BLL.Interfaces;

namespace TGParser.API.Controllers.Messages.ChatShared.Implementations.Preset;

public class SetDefaultPreset(ITelegramBotClient client, 
    IPresetManager presetManager) : BaseTelegramAction, ITextMessage
{
    public string Name => TextMessageNames.SET_DEFAULT_PRESET;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var allPresets = (await presetManager
             .GetAllPresetsByUserIdAsync(UserId)).OrderBy(o => o.ShowedId);

        var keyboard = new InlineKeyboardMarkup(
        [
            allPresets.Select(p => InlineKeyboardButton.WithCallbackData(p.ShowedId.ToString(),
                $"{CallbackQueryNames.SELECT_DEFAULT_PRESET}_{p.ShowedId}_{UserId}"))
        ]);

        await client.SendMessage(
            chatId: ChatId,
            text: "Выберите пресет для поиска",
            replyMarkup: keyboard);
    }
}

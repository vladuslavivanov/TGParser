using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.CallbackQueries;
using TGParser.API.Controllers.Messages.ChatShared.Interfaces;
using TGParser.API.Controllers.Messages.Helpers;
using TGParser.BLL.Interfaces;

namespace TGParser.API.Controllers.Messages.ChatShared.Implementations.Preset;

public class PresetCommand(ITelegramBotClient client,
    IPresetManager presetManager) : BaseTelegramAction, ITextMessage
{
    public string Name => TextMessageNames.PRESETS;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var presets = (await presetManager
            .GetAllPresetsByUserIdAsync(UserId)).OrderBy(o => o.ShowedId)
            .ToList();

        if (!presets.Any())
        {
            await client.SendMessage(ChatId, "❌ Пресеты не найдены",
                replyMarkup: new InlineKeyboardMarkup(
                        InlineKeyboardButton.WithCallbackData("➕ Добавить новый пресет", 
                            CallbackQueryNames.ADD_PRESET)));
            return;
        }

        await client.SendMessage(
            chatId: ChatId,
            text: presets[0].ToString(),
            replyMarkup: ConfigureReplyMarkupHelper
                .ConfigureMarkupForPresets(presets[0].IsSelected, presets[0].ShowedId, 
                presets.Count() >= 2 ? presets[1].ShowedId : null, null )
            );
    }
}

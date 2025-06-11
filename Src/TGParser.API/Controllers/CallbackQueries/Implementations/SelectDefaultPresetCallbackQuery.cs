using Telegram.Bot;
using Telegram.Bot.Types;
using TGParser.API.Controllers.CallbackQueries.Interfaces;
using TGParser.BLL.Interfaces;

namespace TGParser.API.Controllers.CallbackQueries.Implementations;

public class SelectDefaultPresetCallbackQuery(ITelegramBotClient client, 
    IUserPresetManager userPresetManager) : BaseTelegramAction, ICallbackQuery
{
    public string Name => CallbackQueryNames.SELECT_DEFAULT_PRESET;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var splattedData = CallbackQueryData!.Split('_');

        var presetId = splattedData[1];
        var userId = splattedData[2];

        var isCompleted = await userPresetManager.TrySetDefaultPresetAsync(long.Parse(userId), int.Parse(presetId));

        if (isCompleted) 
        {
            await client.EditMessageText(ChatId, (int)BotMessageId!, "Пресет для поиска изменен ✅");
            return;
        }

        await client.EditMessageText(ChatId, (int)BotMessageId!, $"Пресет {presetId} не найден ❌");
    }
}

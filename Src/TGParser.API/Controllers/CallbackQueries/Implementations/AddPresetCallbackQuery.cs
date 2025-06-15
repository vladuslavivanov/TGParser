using Telegram.Bot;
using Telegram.Bot.Types;
using TGParser.API.Controllers.CallbackQueries.Interfaces;
using TGParser.API.Controllers.Messages.Helpers;
using TGParser.BLL.Interfaces;

namespace TGParser.API.Controllers.CallbackQueries.Implementations;

public class AddPresetCallbackQuery(ITelegramBotClient client,
    IPresetManager presetManager) : BaseTelegramAction, ICallbackQuery
{
    public string Name => CallbackQueryNames.ADD_PRESET;

    public async Task Execute(Update update)
    {
        SetContext(update);

        // Добавление нового пресета.
        var presetId = await presetManager.AddPresetToUserAsync(UserId);

        // Получение списка всех пересетов.
        var presets = await presetManager.GetAllPresetsByUserIdAsync(UserId);

        // Модель добавленного пресета.
        var preset = presets.First(p => p.ShowedId == presetId);

        // Отправка сообщения с характеристиками пресета.
        await client.EditMessageText(UserId, (int)BotMessageId!, preset.ToString(),
                replyMarkup: ConfigureReplyMarkupHelper.ConfigureMarkupForPresets(
                    preset.IsSelected, preset.ShowedId, presets.FirstOrDefault(p => p.ShowedId == preset.ShowedId + 1)?.ShowedId,
                    presets.FirstOrDefault(p => p.ShowedId == preset.ShowedId - 1)?.ShowedId));
    }
}

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
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

        var isCompleted = await userPresetManager.TrySetDefaultPresetAsync(UserId, int.Parse(presetId));


        if (isCompleted) 
        {
            var newCallbackData = new InlineKeyboardMarkup();

            var oldCallbackData = 
                update.CallbackQuery!.Message!.ReplyMarkup!.InlineKeyboard;

            foreach (var item in oldCallbackData)
            {
                var callbackData = item.Select(s => s.CallbackData);

                if (!callbackData.Any(str => str!.Contains(CallbackQueryNames.SELECT_DEFAULT_PRESET)))
                    newCallbackData.AddNewRow(item.ToArray());
            }

            await client.EditMessageReplyMarkup(ChatId, MessageId, newCallbackData);

            await client.AnswerCallbackQuery(CallbackQueryId!, "✅ Пресет для поиска изменен", true);
            return;
        }

        await client.EditMessageText(ChatId, (int)BotMessageId!, $"❌ Пресет {presetId} не найден");
    }
}

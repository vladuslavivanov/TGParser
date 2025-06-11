using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.CallbackQueries.Interfaces;

namespace TGParser.API.Controllers.CallbackQueries;

public class CallbackQueryExecutor(IEnumerable<ICallbackQuery> commands, ITelegramBotClient client)
{
    public async Task Execute(Update update)
    {
        if (await HasButtonExpired(update))
            return;

        var data = update.CallbackQuery!.Data;

        var commandName = data!.Split('_')[0];

        var command = commands.FirstOrDefault(f => f.Name == commandName);

        if (command == default) return;

        await command.Execute(update);
    }

    async Task<bool> HasButtonExpired(Update update)
    {
        if (update.CallbackQuery!.Message!.Date.AddMinutes(30) > DateTime.UtcNow)
            return false;

        await client.EditMessageText(update.CallbackQuery!.Message!.Chat.Id, 
            update.CallbackQuery.Message.MessageId,
            "Выполните запрос еще раз. Истек срок действия сообщения ⏰", 
            replyMarkup: InlineKeyboardMarkup.Empty());

        return true;
    }
}

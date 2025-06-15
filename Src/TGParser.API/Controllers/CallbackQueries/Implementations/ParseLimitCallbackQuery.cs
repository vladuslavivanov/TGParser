using Telegram.Bot;
using Telegram.Bot.Types;
using TGParser.API.Controllers.CallbackQueries.Interfaces;

namespace TGParser.API.Controllers.CallbackQueries.Implementations;

public class ParseLimitCallbackQuery(
    ITelegramBotClient client) : BaseTelegramAction, ICallbackQuery
{
    public string Name => CallbackQueryNames.PARSE_LIMIT;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var dataArray = CallbackQueryData!.Split('_');

        var quantityAdv = int.Parse(dataArray[1]);
        var userId = dataArray[2];
        
        await client.EditMessageText(ChatId, (int)BotMessageId!, "Выберите тип запроса", 
            replyMarkup: CallbackQueryHelper.SelectActionPreParse(userId, quantityAdv));
    }
}

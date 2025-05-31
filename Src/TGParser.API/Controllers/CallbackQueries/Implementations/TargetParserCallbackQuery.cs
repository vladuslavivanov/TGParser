using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.CallbackQueries.Interfaces;

namespace TGParser.API.Controllers.CallbackQueries.Implementations;

public class TargetParserCallbackQuery(ITelegramBotClient client) : BaseTelegramAction, ICallbackQuery
{
    public string Name => CallbackQueryNames.TARGET_PARSER;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var callbackMessage = CallbackQueryData!.Split('_')[1];

        if (callbackMessage == "myself")
        {
            await client.EditMessageText(ChatId, (int)BotMessageId!, "#️⃣ Выберите количество объявлений", 
                replyMarkup: Static.GetParseLimitInlineKeyboardMarkup(UserId));
        }
        else if(callbackMessage == "other")
        {
            var requestUserButton = new KeyboardButton
            {
                Text = "Выбрать пользователя",
                RequestUsers = new KeyboardButtonRequestUsers
                {
                    RequestId = UsersSharedRequestIds.SELECT_USER_FOR_PARSE,
                    UserIsBot = false,
                    RequestUsername = true
                },
            };

            var replyKeyboard = new ReplyKeyboardMarkup([[requestUserButton]])
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true,
            };

            await client.DeleteMessage(ChatId, (int)BotMessageId!);

            await client.SendMessage(ChatId, "Выберите пользователя", replyMarkup: replyKeyboard);
        }
    }
}

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.Messages.ChatShared.Interfaces;
using TGParser.API.Controllers.CallbackQueries;
using TGParser.BLL.Interfaces;

namespace TGParser.API.Controllers.Messages.ChatShared.Implementations;

public class SelectUserForParseMessage(ITelegramBotClient client, IUserManager userManager) : BaseTelegramAction, IUsersSharedMessage
{
    public int RequestId => UsersSharedRequestIds.SELECT_USER_FOR_PARSE;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var userId = update.Message!.UsersShared!.Users.First().UserId;
        var username = update.Message!.UsersShared!.Users.First().Username;

        var userExists = await userManager.CheckUserExists(userId);

        if (!userExists)
        {
            await client.SendMessage(
            chatId: ChatId,
            text: $"❌ Пользователь не зарегистрирован в боте",
            replyMarkup: new ReplyKeyboardRemove());
            return;
        }

        await client.SendMessage(
            chatId: ChatId,
            text: $"🛫 Объявления будут отправляться:\n{userId} - @{username}",
            replyMarkup: new ReplyKeyboardRemove()
        );

        var message = await client.SendMessage(ChatId, "#️⃣ Выберите количество объявлений", 
            replyMarkup: CallbackQueryHelper.GetParseLimitInlineKeyboardMarkup(userId));
    }
}

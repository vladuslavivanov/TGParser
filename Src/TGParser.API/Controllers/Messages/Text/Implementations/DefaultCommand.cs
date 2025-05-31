using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.Messages.ChatShared;
using TGParser.API.Controllers.Messages.ChatShared.Interfaces;
using TGParser.BLL.Interfaces;

namespace TGParser.API.Controllers.Messages.ChatShared.Implementations;

public class DefaultCommand(ITelegramBotClient client,
    IUserManager userManager) : BaseTelegramAction, ITextMessage
{
    public string Name => TextMessageNames.HOME;

    public async Task Execute(Update update)
    {
        SetContext(update);

        await userManager.AddIfNotExistsAsync(UserId);

        var keyboard = new ReplyKeyboardMarkup(
        [
            [TextMessageNames.SEARCH_WALLAPOP],
            [TextMessageNames.HELP, TextMessageNames.PROFILE],
            //[CommandNames.ABOUT]
        ])
        {
            ResizeKeyboard = true,
        };

        var message = await client.SendMessage(
                chatId: ChatId,
                text: "Выберите пункт меню",
                replyMarkup: keyboard
                );
    }
}
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.Messages.ChatShared;
using TGParser.API.Controllers.Messages.ChatShared.Interfaces;
using TGParser.Configuration;

namespace TGParser.API.Controllers.Messages.ChatShared.Implementations;

public class HelpCommand(ITelegramBotClient client) : BaseTelegramAction, ITextMessage
{
    static readonly string SUPPORT = ConfigurationStorage.GetSupport();

    public string Name => TextMessageNames.HELP;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var keyboard = new ReplyKeyboardMarkup(
        [
            [TextMessageNames.HOME]
        ])
        {
            ResizeKeyboard = true,
        };

        var message = await client.SendMessage(
                chatId: ChatId,
                text: SUPPORT,
                replyMarkup: keyboard
                );
    }
}

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.Commands.Interfaces;
using TGParser.Configuration;

namespace TGParser.API.Controllers.Commands.Implementations;

public class HelpCommand(ITelegramBotClient client) : BaseCommand, ICommand
{
    static readonly string SUPPORT = ConfigurationStorage.GetSupport();

    public string Name => CommandNames.HELP;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var keyboard = new ReplyKeyboardMarkup(
        [
            [CommandNames.HOME]
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

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.Commands.Interfaces;
using TGParser.BLL.Interfaces;

namespace TGParser.API.Controllers.Commands.Implementations;

public class DefaultCommand(ITelegramBotClient client, 
    IUserManager userManager) : BaseCommand, ICommand
{
    public string Name => CommandNames.HOME;

    public async Task Execute(Update update)
    {
        SetContext(update);

        await userManager.AddIfNotExistsAsync(UserId);

        var keyboard = new ReplyKeyboardMarkup(
        [
            [CommandNames.SEARCH_WALLAPOP],
            [CommandNames.HELP, CommandNames.PROFILE],
            [CommandNames.ABOUT]
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
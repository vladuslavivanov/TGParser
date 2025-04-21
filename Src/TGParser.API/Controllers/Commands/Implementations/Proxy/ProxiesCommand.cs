using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.Commands.Interfaces;
using TGParser.BLL.Interfaces;

namespace TGParser.API.Controllers.Commands.Implementations.Proxy;

public class ProxiesCommand(ITelegramBotClient client, IProxyManager proxyManager) 
    : BaseCommand, ICommand
{
    public string Name => CommandNames.PROXIES;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var keyboard = new ReplyKeyboardMarkup(
        [
            [CommandNames.ADD_PROXY],
            [CommandNames.EDIT_PROXY, CommandNames.REMOVE_PROXY],
            [CommandNames.TEST_PROXY],
            [CommandNames.HOME]
        ])
        {
            ResizeKeyboard = true,
        };

        var proxies = (await proxyManager.GetAllProxies(UserId))
            .OrderBy(o => o.ShowedId);

        if (!proxies.Any())
        {
            await client.SendMessage(
                ChatId,
                "⚠️ Вы ещё не добавили ни одного прокси-сервера",
                replyMarkup: keyboard);
            return;
        }

        foreach (var item in proxies)
        {
            await client.SendMessage(
                ChatId,
                item.ToString(),
                replyMarkup: keyboard);
        }
    }
}
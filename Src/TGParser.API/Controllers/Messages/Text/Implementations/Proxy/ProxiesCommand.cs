using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.Messages.ChatShared;
using TGParser.API.Controllers.Messages.ChatShared.Interfaces;
using TGParser.BLL.Interfaces;

namespace TGParser.API.Controllers.Messages.ChatShared.Implementations.Proxy;

public class ProxiesCommand(ITelegramBotClient client, IProxyManager proxyManager)
    : BaseTelegramAction, ITextMessage
{
    public string Name => TextMessageNames.PROXIES;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var keyboard = new ReplyKeyboardMarkup(
        [
            [TextMessageNames.ADD_PROXY],
            [TextMessageNames.EDIT_PROXY, TextMessageNames.REMOVE_PROXY],
            [TextMessageNames.TEST_PROXY],
            [TextMessageNames.HOME]
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
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.CallbackQueries;
using TGParser.API.Controllers.Messages.ChatShared.Interfaces;
using TGParser.API.Services.Interfaces;

namespace TGParser.API.Controllers.Messages.ChatShared.Implementations.Parsing;

public class SearchWallapopCommand(
    IUserService userService,
    ITelegramBotClient client) : BaseTelegramAction, ITextMessage
{
    public string Name => TextMessageNames.SEARCH_WALLAPOP;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var isSubscribed = await userService.IsUserSubscribed(UserId);

        if (!isSubscribed)
        {
            await client.SendMessage(ChatId,
                "Для продолжения использования парсера необходимо оплатить подписку");

            return;
        }

        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("🧑‍💻 Себе 🧑‍💻", $"{CallbackQueryNames.TARGET_PARSER}_myself"),
                InlineKeyboardButton.WithCallbackData("👥 Другому 👥", $"{CallbackQueryNames.TARGET_PARSER}_other"),
            }
        });

        await client.SendMessage(ChatId, "Кому парсить объявления?", replyMarkup: keyboard);
    }
}

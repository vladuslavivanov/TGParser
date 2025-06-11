using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

namespace TGParser.API.Controllers;

public class BaseTelegramAction
{
    protected long ChatId { get; private set; }
    protected int MessageId { get; private set; }
    protected long UserId { get; private set; }
    protected Message? Message { get; private set; }

    protected string UserName { get; private set; }

    protected int? BotMessageId { get; private set; }
    protected string? CallbackQueryData { get; private set; }
    protected string? CallbackQueryId { get; private set; }

    protected void SetContext(Update update)
    {
        switch (update.Type)
        {
            case UpdateType.Message:
                ChatId = update.Message!.Chat.Id;
                MessageId = update.Message.MessageId;
                UserId = update.Message.From!.Id;
                UserName = update.Message!.From.Username ?? "";
                Message = update.Message;
                break;
            case UpdateType.CallbackQuery:
                ChatId = update.CallbackQuery!.Message!.Chat.Id;
                MessageId = update.CallbackQuery.Message.MessageId;
                UserId = update.CallbackQuery.From.Id;
                UserName = update.CallbackQuery.From.Username ?? "";
                Message = update.CallbackQuery.Message;
                BotMessageId = update.CallbackQuery.Message.MessageId;
                CallbackQueryData = update.CallbackQuery.Data;
                CallbackQueryId = update.CallbackQuery.Id;
                break;
            default:
                break;
        }
    }
}

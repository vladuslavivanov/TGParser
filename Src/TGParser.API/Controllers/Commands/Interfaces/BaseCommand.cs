using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TGParser.API.Controllers.Commands.Interfaces;

public abstract class BaseCommand
{
    protected long ChatId { get; private set; }
    protected int MessageId { get; private set; }
    protected long UserId { get; private set; }
    protected Message? Message { get; private set; }

    protected void SetContext(Update update)
    {
        switch (update.Type)
        {
            case UpdateType.Message:
                ChatId = update.Message!.Chat.Id;
                MessageId = update.Message.MessageId;
                UserId = update.Message!.From!.Id;
                Message = update.Message!;
                break;
            case UpdateType.CallbackQuery:
                ChatId = update.CallbackQuery!.Message!.Chat.Id;
                MessageId = update.CallbackQuery.Message.MessageId;
                UserId = update.CallbackQuery.Message!.Chat.Id;
                break;
            default:
                break;
        }

        
    }
}

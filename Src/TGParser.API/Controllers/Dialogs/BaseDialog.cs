using MassTransit;
using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.Commands.Interfaces;
using TGParser.API.MassTransit.Requsted;
using TGParser.API.Services.Interfaces;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Dialogs;

public abstract class BaseDialog<T>(IBus bus, IDialogService dialogService, ITelegramBotClient client): BaseCommand where T : BaseContext
{
    protected static ConcurrentDictionary<long, T> _dialogContexts = new();

    Message? LastSendedMessage;

    public async Task<bool> TryHandleUserLeaveAsync(DialogState? nextState = null, string? nextCommandName = null)
    {
        if (Message!.Text != EditingNames.LEAVE)
            return false;

        if (nextState != null)
        {
            _dialogContexts[UserId].DialogState = (DialogState)nextState;
            Message!.Text = "";
            await bus.Publish(new RequestDialogCommand(Message));
        }

        else if (nextCommandName != null)
        {
            dialogService.UserFinalDialog(UserId);
            _dialogContexts.TryRemove(UserId, out var context);
            await bus.Publish(new RequestMessageCommand(new() { Message = Message }, nextCommandName));
        }

        return true;
    }

    public async Task SendMessage(string text, ReplyMarkup? replyMarkup = null)
    {
        LastSendedMessage = await client.SendMessage(ChatId, text, replyMarkup: replyMarkup);
    }
}

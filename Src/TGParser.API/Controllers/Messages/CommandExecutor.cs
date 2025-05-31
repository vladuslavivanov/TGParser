using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TGParser.API.Controllers.Messages.ChatShared;
using TGParser.API.Controllers.Messages.ChatShared.Interfaces;
using TGParser.API.Controllers.Messages.Interfaces;

namespace TGParser.API.Controllers.Messages;

public class CommandExecutor
{
    IEnumerable<IMessage> messages;
    IMessage defaultCommand;

    public CommandExecutor(IEnumerable<IMessage> commands)
    {
        messages = commands;

        var textMessages = commands.OfType<ITextMessage>();

        defaultCommand = textMessages.First(f => f.Name == TextMessageNames.HOME);
    }

    public async Task Execute(Update update)
    {
        IMessage? impl = default;

        switch (update.Message!.Type)
        {
            case MessageType.Text:
                var textMessages = messages.OfType<ITextMessage>();
                impl = textMessages.FirstOrDefault(f => f.Name == update.Message!.Text)
                    ?? defaultCommand;
                break;
            case MessageType.UsersShared:
                var chatSharedMessages = messages.OfType<IUsersSharedMessage>();
                impl = chatSharedMessages.FirstOrDefault(f => f.RequestId == update.Message.UsersShared!.RequestId);
                break;
        }

        if (impl != default)
            await impl.Execute(update);
    }

    public async Task DispatchAsync(string commandName, Update update)
    {
        var textMessages = messages.OfType<ITextMessage>();
        var command =
            textMessages.FirstOrDefault(f => f.Name == commandName)
            ?? defaultCommand;

        await command.Execute(update);
    }
}


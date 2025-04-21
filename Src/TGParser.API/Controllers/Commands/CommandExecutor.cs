using Telegram.Bot.Types;
using TGParser.API.Controllers.Commands.Interfaces;

namespace TGParser.API.Controllers.Commands;

public class CommandExecutor
{
    IEnumerable<ICommand> commands;
    ICommand defaultCommand;

    public CommandExecutor(IEnumerable<ICommand> commands)
    {
        this.commands = commands;
        defaultCommand = commands.First(f => f.Name == CommandNames.HOME);
    }

    public async Task Execute(Update update)
    {
        var command =
            commands.FirstOrDefault(f => f.Name == update.Message?.Text)
            ?? defaultCommand;

        await command.Execute(update);
    }

    public async Task DispatchAsync(string commandName, Update update)
    {
        var command =
            commands.FirstOrDefault(f => f.Name == commandName)
            ?? defaultCommand;

        await command.Execute(update);
    }
}


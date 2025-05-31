using Telegram.Bot.Types;
using TGParser.API.Controllers.CallbackQueries.Interfaces;

namespace TGParser.API.Controllers.CallbackQueries;

public class CallbackQueryExecutor(IEnumerable<ICallbackQuery> commands)
{
    public async Task Execute(Update update)
    {
        var data = update.CallbackQuery!.Data;

        var commandName = data!.Split('_')[0];

        var command = commands.FirstOrDefault(f => f.Name == commandName);

        if (command == default) return;

        await command.Execute(update);
    }
}

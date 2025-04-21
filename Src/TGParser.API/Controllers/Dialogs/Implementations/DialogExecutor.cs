using Telegram.Bot.Types;
using TGParser.API.Controllers.Dialogs.Interfaces;
using TGParser.API.Services.Interfaces;

namespace TGParser.API.Controllers.Dialogs.Implementations;

public class DialogExecutor(IEnumerable<IDialog> dialogs, IDialogService dialogService) : IDialogExecutor
{
    public async Task ExecuteDialog(Message message)
    {
        var userId = message.From!.Id;
        var dialogType = 
            dialogService.GetUserDialogType(userId);

        var dialogToExecute = 
            dialogs.FirstOrDefault(d => d.DialogType == dialogType);

        if (dialogToExecute != default)
            await dialogToExecute.Execute(message);
    }
}

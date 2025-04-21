using Telegram.Bot.Types;

namespace TGParser.API.Controllers.Dialogs.Interfaces;

/// <summary>
/// Интерфейс для маршрутизатора диалогов.
/// </summary>
public interface IDialogExecutor
{
    Task ExecuteDialog(Message message);
}

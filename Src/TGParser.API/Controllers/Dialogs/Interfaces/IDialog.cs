using Telegram.Bot.Types;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Dialogs.Interfaces;

/// <summary>
/// Интерфейс для всех диалогов с пользователями.
/// </summary>
public interface IDialog
{
    DialogType DialogType { get; }
    Task Execute(Message message);
}

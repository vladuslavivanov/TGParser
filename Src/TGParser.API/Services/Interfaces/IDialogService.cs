using TGParser.Core.Enums;

namespace TGParser.API.Services.Interfaces;

/// <summary>
/// Сервис диалогов.
/// </summary>
public interface IDialogService
{
    bool CheckUserInDialog(long userId);
    void SetUserDialog(long userId, DialogType dialogType);
    void UserFinalDialog(long userId);
    DialogType GetUserDialogType(long userId);
}

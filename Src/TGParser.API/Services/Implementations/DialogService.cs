using System.Collections.Concurrent;
using TGParser.API.Services.Interfaces;
using TGParser.Core.Enums;

namespace TGParser.API.Services.Implementations;

public class DialogService : IDialogService
{
    ConcurrentDictionary<long, DialogType> userDialogs = new();

    public bool CheckUserInDialog(long userId) => 
        userDialogs.TryGetValue(userId, out _);

    public void SetUserDialog(long userId, DialogType dialogType)
    {
        userDialogs[userId] = dialogType;
    }

    public void UserFinalDialog(long userId)
    {
        userDialogs.TryRemove(userId, out _);
    }

    public DialogType GetUserDialogType(long userId)
    {
        userDialogs.TryGetValue(userId, out var dialogType);
        return dialogType;
    }

}

using MassTransit;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.Dialogs.Interfaces;
using TGParser.API.Controllers.Messages.ChatShared;
using TGParser.API.Services.Interfaces;
using TGParser.BLL.Interfaces;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Dialogs.Implementations.Preset;

public class RemovePresetDialog(ITelegramBotClient client, IPresetManager presetManager, IBus bus, IDialogService dialogService)
    : BaseDialog<BaseContext>(bus, dialogService, client), IDialog
{
    public DialogType DialogType => DialogType.RemovePreset;

    public async Task Execute(Message message)
    {
        SetContext(new() { Message = message });

        _dialogContexts.TryGetValue(UserId, out var dialogContext);

        // Начало диалога.
        if (dialogContext == default)
        {
            _dialogContexts[UserId] = new();
            await SendMenu();
            return;
        }

        if (dialogContext.DialogState == DialogState.FirstStep)
        {
            if (await TryHandleUserLeaveAsync(nextCommandName: TextMessageNames.PRESETS))
                return;

            await TryRemovePreset();

            await SendMenu();
        }
    }

    async Task SendMenu()
    {
        var allPresets = (await presetManager
            .GetAllPresetsByUserIdAsync(UserId)).OrderBy(o => o.ShowedId);

        var keyboard = new ReplyKeyboardMarkup(
        [
            allPresets.Select(p => new KeyboardButton(p.ShowedId.ToString())),
            [EditingNames.LEAVE]
        ])
        {
            ResizeKeyboard = true,
        };

        await client.SendMessage(
            chatId: ChatId,
            text: "Выберите пресет для удаления",
            replyMarkup: keyboard);
    }

    async Task TryRemovePreset()
    {
        var parsed = int.TryParse(Message!.Text, out var selectedPreset);
        if (!parsed)
        {
            await client.SendMessage(ChatId, "Введите число!");
            return;
        }

        await presetManager.RemovePresetByShowedIdAsync(UserId, selectedPreset);
    }
}
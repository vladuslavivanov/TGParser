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

public class SetDefaultPresetDialog(ITelegramBotClient client,
    IBus bus,
    IDialogService dialogService,
    IPresetManager presetManager, IUserPresetManager userPresetManager)
    : BaseDialog<BaseContext>(bus, dialogService, client), IDialog
{
    public DialogType DialogType => DialogType.SetDefaultPreset;

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

            if (!await TrySetPreset())
            {
                await SendMenu();
            }
            else
            {
                Message!.Text = EditingNames.LEAVE;
                await TryHandleUserLeaveAsync(nextCommandName: TextMessageNames.PRESETS);
                return;
            }
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
            text: "Выберите пресет для поиска",
            replyMarkup: keyboard);
    }

    async Task<bool> TrySetPreset()
    {
        var parsed = int.TryParse(Message!.Text, out var selectedPreset);
        if (!parsed)
        {
            await client.SendMessage(ChatId, "Введите число!");
            return parsed;
        }

        var result = await userPresetManager
            .TrySetDefaultPresetAsync(UserId, selectedPreset);

        return result;
    }
}

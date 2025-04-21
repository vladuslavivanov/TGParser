using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.Commands.Interfaces;
using TGParser.API.Services;
using TGParser.BLL.Interfaces;

namespace TGParser.API.Controllers.Commands.Implementations.Preset;

public class PresetCommand(ITelegramBotClient client,
    IPresetManager presetManager) : BaseCommand, ICommand
{
    public string Name => CommandNames.PRESETS;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var keyboard = new ReplyKeyboardMarkup(
        [
            [CommandNames.ADD_PRESET],
            [CommandNames.EDIT_PRESET, CommandNames.REMOVE_PRESET],
            [CommandNames.HOME]
        ])
        {
            ResizeKeyboard = true,
        };

        var presets = (await presetManager
            .GetAllPresetsByUserIdAsync(UserId)).OrderBy(o => o.ShowedId);

        //UserMessageDeletionCacheService.AddMessage(ChatId, MessageId);

        if (presets == default || !presets.Any())
        {
            var message = await client.SendMessage(
               chatId: ChatId,
               text: "🎛️ У Вас нет пресетов",
               replyMarkup: keyboard
            );

            //await UserMessageDeletionCacheService.ClearMessages(client, ChatId);

            //UserMessageDeletionCacheService.AddMessage(ChatId, message.Id);
            return;
        }

        var messages = new List<Message>();

        foreach (var item in presets)
        {
            var message = await client.SendMessage(
                chatId: ChatId,
                text: item.ToString(),
                replyMarkup: keyboard
                );

            messages.Add(message);
        }

        //await UserMessageDeletionCacheService.ClearMessages(client, ChatId);

        //messages.ForEach(m => UserMessageDeletionCacheService.AddMessage(ChatId, m.Id));

    }
}

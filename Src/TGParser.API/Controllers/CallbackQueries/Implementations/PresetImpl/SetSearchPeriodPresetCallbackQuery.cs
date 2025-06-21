using Telegram.Bot;
using Telegram.Bot.Types;
using TGParser.API.Controllers.CallbackQueries.Interfaces;
using TGParser.API.Controllers.Helpers;
using TGParser.BLL.Interfaces;
using TGParser.Core.Enums;
using TGParser.Core.Enums.Presets;

namespace TGParser.API.Controllers.CallbackQueries.Implementations.PresetImpl;

public class SetSearchPeriodPresetCallbackQuery(ITelegramBotClient client,
    IPresetManager presetManager) : BaseTelegramAction, ICallbackQuery
{
    public string Name => CallbackQueryNames.SET_SEARCH_PERIOD_PRESET;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var splitData = CallbackQueryData!.Split('_').ToList();

        var presetId = int.Parse(splitData[1]);

        var step = Enum.Parse<SetSearchPeriodStep>(splitData[2]);

        Enum.TryParse<PeriodSearch>(splitData.ElementAtOrDefault(3), out var newPeriodSearch);

        switch (step)
        {
            case SetSearchPeriodStep.ShowVariously:
                await ShowVariously(presetId);
                break;
            case SetSearchPeriodStep.SetSearchPeriod:
                await SetSearchPeriod(presetId, newPeriodSearch);
                break;
        }        
    }

    async Task ShowVariously(int presetId)
    {
        var keyboardMarkup = CallbackQueryHelper.ConfigurePeriodSearchKeyboard(presetId);

        await client.EditMessageReplyMarkup(ChatId, (int)BotMessageId!, keyboardMarkup);
    }

    async Task SetSearchPeriod(int presetId, PeriodSearch newPeriodSearch)
    {
        await presetManager.SetSearchPeriod(UserId, presetId, newPeriodSearch);

        await UpdateMessageHelper.UpdateUserPreset(client, presetManager, UserId, presetId, (int)BotMessageId!, Message!.ReplyMarkup);

        await client.AnswerCallbackQuery(CallbackQueryId!, "✅ Готово");
    }
}

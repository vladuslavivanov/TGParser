using MassTransit;
using Telegram.Bot;
using Telegram.Bot.Types;
using TGParser.API.Controllers.CallbackQueries.Interfaces;
using TGParser.API.MassTransit.Requsted;
using TGParser.BLL.Interfaces;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.CallbackQueries.Implementations.PresetImpl;

public class SetSearchPeriodPresetCallbackQuery(IBus bus,
    IPresetManager presetManager) : BaseTelegramAction, ICallbackQuery
{
    public string Name => CallbackQueryNames.SET_SEARCH_PERIOD_PRESET;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var splitData = CallbackQueryData!.Split('_');

        var presetId = splitData[1];
        var newPeriodSearch = splitData[2];

        await presetManager.SetSearchPeriod(UserId, int.Parse(presetId), Enum.Parse<PeriodSearch>(newPeriodSearch));

        update.CallbackQuery!.Data = $"{CallbackQueryNames.SHOW_PRESET}_{presetId}";

        await bus.Publish(new RequestBeginCallbackQuery(update));
    }
}

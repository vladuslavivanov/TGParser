using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TGParser.API.Controllers.CallbackQueries.Interfaces;
using TGParser.API.Services.Interfaces;
using TGParser.BLL.Interfaces;
using TGParser.Core.DTO.Wallapop;

namespace TGParser.API.Controllers.CallbackQueries.Implementations;

public class SearchAdvCallbackQuery(
    ITelegramBotClient client,
    IProxyService proxyService,
    IUserPresetManager userPresetManager,
    ISearchWallapopService searchWallapopService) : BaseTelegramAction, ICallbackQuery
{
    public string Name => CallbackQueryNames.SEARCH_ADV;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var dataArray = CallbackQueryData!.Split("_");

        var quantityAdv = int.Parse(dataArray[1]);
        var userId = long.Parse(dataArray[2]);
        var query = dataArray[3] == "-" ? "" : dataArray[3];

        var proxy = await proxyService.GetAvailableProxyByUserId(UserId);

        if (proxy == default)
        {
            await client.EditMessageText(ChatId, (int)BotMessageId!
                , "❌ Не было найдено рабочих прокси. Добавьте рабочий прокси в настройках");
            return;
        }

        var selectedPreset = await userPresetManager.GetSelectedPresetAsync(UserId);

        await client.EditMessageText(ChatId, (int)BotMessageId!, "✅ Парсинг начался");

        int countAdv = 0;

        string nextPage = "";

        Root? result = null;

        while(countAdv < quantityAdv)
        {
            if (!string.IsNullOrEmpty(nextPage))
                result = await searchWallapopService.SearchNext(nextPage, proxy);
            else
                result = await searchWallapopService.Search(query, proxy, selectedPreset);

            var items = result?.Data.Section.Payload.Items;

            if (items?.Count == default)
            {
                if (countAdv == quantityAdv - quantityAdv)
                    await client.SendMessage(ChatId,
                        "❌ По вашему запросу ничего не найдено");
                else if (countAdv != quantityAdv)
                    await client.SendMessage(ChatId,
                        $"❌ По вашему запросу найдено {countAdv} объявлений");
                break;
            }

            nextPage = result!.Meta.NextPage;

            CancellationTokenSource ctsTime = new() { };
            CancellationTokenSource ctsQuantity = new();

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ctsTime.Token, ctsQuantity.Token);
            linkedCts.CancelAfter(TimeSpan.FromSeconds(30));

            try
            {
                await Parallel.ForEachAsync(items!, linkedCts.Token, async (item, ct) =>
                {
                    if (ct.IsCancellationRequested)
                        return;

                    var result = await searchWallapopService.FilterItem(item, selectedPreset, proxy);
                    if (result != default)
                    {
                        Interlocked.Increment(ref countAdv);
                        await client.SendPhoto(userId,
                                item.Images.FirstOrDefault()?.Urls.Small ?? "",
                                result.ToString(),
                                ParseMode.Html,
                                cancellationToken: ct
                        );
                        if (countAdv > quantityAdv)
                            linkedCts.Cancel();
                    }
                });
            }
            catch (Exception ex)
            {

            }
        }


        await client.SendMessage(ChatId, "✅ Парсинг завершен");
    }
}

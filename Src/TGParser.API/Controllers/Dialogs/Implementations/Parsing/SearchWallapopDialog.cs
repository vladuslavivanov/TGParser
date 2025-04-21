using MassTransit;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.Commands;
using TGParser.API.Controllers.Dialogs.Contexts;
using TGParser.API.Controllers.Dialogs.Interfaces;
using TGParser.API.Services.Interfaces;
using TGParser.BLL.Interfaces;
using TGParser.Core.Consts;
using TGParser.Core.DTO;
using TGParser.Core.DTO.Wallapop;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Dialogs.Implementations.Parsing;

public class SearchWallapopDialog(
    ITelegramBotClient client, 
    IBus bus, 
    IDialogService dialogService,
    ISearchWallapopService searchWallapopService,
    IProxyManager proxyManager,
    IProxyService proxyService,
    IUserPresetManager userPresetManager) 
    : BaseDialog<SearchWallapopContext>(bus, dialogService, client), IDialog
{
    public DialogType DialogType => DialogType.SearchWallapop;

    public async Task Execute(Message message)
    {
        SetContext(new() { Message = message });

        if (await TryHandleUserLeaveAsync(nextCommandName: CommandNames.HOME))
        {
            return;
        }

        var userProxies = await proxyManager.GetAllProxies(UserId);

        var selectedPreset = await userPresetManager.GetSelectedPresetAsync(UserId);

        if (userProxies.Count() == default)
        {
            await client.SendMessage(ChatId, 
                "Для работы парсера необходимо указать прокси сервер");
            Message!.Text = EditingNames.LEAVE;
            await TryHandleUserLeaveAsync(nextCommandName: CommandNames.HOME);
        }

        try
        {
            await ProxyIsEnabled(userProxies.First());
        }
        catch (Exception ex)
        {
            await client.SendMessage(ChatId,
                "Ошибка проверки прокси! Возможно, прокси следует заменить");
            Message.Text = EditingNames.LEAVE;
            await TryHandleUserLeaveAsync(nextCommandName: CommandNames.HOME);
            return;
        }

        _dialogContexts.TryGetValue(UserId, out var dialogContext);

        // Начало диалога.
        if (dialogContext == default)
        {
            _dialogContexts[UserId] = new();
            await SendSearchMenu();
            return;
        }

        List<ViewItemDto>? filteredItems = new();

        Root? result;

        if (dialogContext.DialogState == DialogState.FirstStep)
        {
            if (message.Text! == EditingNames.Search.WITHOUT_KEYWORDS)
                message.Text = "";

            result = await searchWallapopService.Search(message.Text!, userProxies.First(), selectedPreset);

            dialogContext.DialogState = DialogState.SecondStep;
        }
        else
        {
            if (Message!.Text != EditingNames.Search.NEXT_PAGE)
                return;

            result = await searchWallapopService.SearchNext(dialogContext.LinkToNextPage, userProxies.First());            
        }

        dialogContext.LinkToNextPage = result.Meta.NextPage;

        var items = result.Data.Section.Payload.Items;

        if (items.Count == default)
        {
            await client.SendMessage(ChatId, 
                "По вашему запросу ничего не найдено");
            Message.Text = EditingNames.LEAVE;
            await TryHandleUserLeaveAsync(nextCommandName: CommandNames.HOME);
            return;
        }

        CancellationTokenSource cts = new() {  };
        cts.CancelAfter(TimeSpan.FromSeconds(30));

        try
        {
            await Parallel.ForEachAsync(items, cts.Token, async (item, ct) =>
            {
                var result = await searchWallapopService.FilterItem(item, selectedPreset, userProxies.First());
                if (result != default)
                {
                    await client.SendPhoto(ChatId,
                            item.Images.FirstOrDefault()?.Urls.Small ?? "",
                            result.ToString(),
                            ParseMode.Html,
                            replyMarkup: new ReplyKeyboardRemove()
                    );
                }
            });
        }
        catch(Exception ex)
        {

        }

        var keyboard = new ReplyKeyboardMarkup(
        [
            [EditingNames.Search.NEXT_PAGE],
            [EditingNames.LEAVE]
        ])
        {
            ResizeKeyboard = true,
        };

        await client.SendMessage(ChatId,
            "Выберите действие",
            replyMarkup: keyboard);
    }

    async Task SendSearchMenu()
    {
        var keyboard = new ReplyKeyboardMarkup(
        [
            [EditingNames.Search.WITHOUT_KEYWORDS],
            [EditingNames.LEAVE]
        ])
        {
            ResizeKeyboard = true,
        };

        await client.SendMessage(
            chatId: ChatId,
            text: "Введите запрос для поиска",
            replyMarkup: keyboard);
    }

    async Task<bool> ProxyIsEnabled(ProxyDto proxy)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        var result = await proxyService.SendRequestThroughProxy(BotConstants.CHECK_PROXY_API, proxy, cts.Token);
        if (result != proxy.IP)
        {
            return false;
        }
        return true;
    }
}

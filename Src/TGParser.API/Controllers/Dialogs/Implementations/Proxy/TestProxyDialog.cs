using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.Commands;
using TGParser.API.Controllers.Dialogs.Interfaces;
using TGParser.API.Services.Interfaces;
using TGParser.BLL.Interfaces;
using TGParser.Core.Enums;
using MassTransit;
using TGParser.Core.Consts;
using TGParser.Core.DTO;

namespace TGParser.API.Controllers.Dialogs.Implementations.Proxy;

public class TestProxyDialog(ITelegramBotClient client,
    IBus bus, 
    IDialogService dialogService, 
    IProxyManager proxyManager, IProxyService proxyService) : 
    BaseDialog<BaseContext>(bus, dialogService, client), IDialog
{
    public DialogType DialogType => DialogType.TestProxy;

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

        // На данном этапе пользователь уже выбрал сущность.
        if (dialogContext.DialogState == DialogState.FirstStep)
        {
            if (await TryHandleUserLeaveAsync(nextCommandName: CommandNames.PROXIES)) return;

            var isParsed = int.TryParse(message.Text, out var idEntity);

            if (isParsed == false)
            {
                await SendMenu();
                return;
            }

            var proxy = await proxyManager.GetProxyByShowedIdAsync(UserId, idEntity);

            if (proxy == default)
            {
                await SendMenu();
                return;
            }

            if (proxy.ProxyType == ProxyType.SOCKS5)
                await CheckProxy(proxy);
            
            Message!.Text = EditingNames.LEAVE;

            await TryHandleUserLeaveAsync(nextCommandName: CommandNames.PROXIES);
            
            return;
        }
    }

    async Task SendMenu()
    {
        var allPresets = (await proxyManager.GetAllProxies(UserId)).OrderBy(o => o.ShowedId);

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
            text: "Выберите прокси для тестирования",
            replyMarkup: keyboard);
    }

    async Task<bool> CheckProxy(ProxyDto proxy)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        string ipUnderProxy = "";

        try
        {
            ipUnderProxy = await proxyService.SendRequestThroughProxy(BotConstants.CHECK_PROXY_API, proxy, cts.Token);
        }
        catch (TaskCanceledException) when (cts.IsCancellationRequested)
        {
            await client.SendMessage(ChatId, "🛑 Ошибка! Сервис ipify не отправил ответ в течении 5 секунд. Запрос отменён по таймауту!");
            return false;
        }
        catch (Exception ex)
        {
            await client.SendMessage(ChatId, $"🛑 Ошибка! {ex.Message}");
            return false;
        }

        await client.SendMessage(ChatId, $"✅ IP через прокси: {ipUnderProxy}");
        return true;
    }
}

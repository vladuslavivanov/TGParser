using System.Net;
using Telegram.Bot.Types;
using TGParser.API.Controllers.Dialogs;
using TGParser.API.Controllers.Messages.ChatShared;
using TGParser.API.Services.Interfaces;
using TGParser.BLL.Interfaces;
using TGParser.Core.Consts;
using TGParser.Core.DTO;

namespace TGParser.API.Services.Implementations;

public class ProxyService(IProxyManager proxyManager) : IProxyService
{
    public async Task<string> SendRequestThroughProxy(string url, ProxyDto proxy, CancellationToken? token = null)
    {
        var webProxy = new WebProxy
        {
            Address = new Uri($"socks5://{proxy.IP}:{proxy.Port}"),
            Credentials = new NetworkCredential(proxy.UserName, proxy.Password)
        };

        var handler = new HttpClientHandler
        {
            Proxy = webProxy
        };

        var httpClient = new HttpClient(handler);

        httpClient.DefaultRequestHeaders.Add("X-DeviceOS", "0");

        var response = await httpClient.SendAsync(new(HttpMethod.Get, url), token ?? new());

        var str = await response.Content.ReadAsStringAsync();

        return str ?? "";
    }

    public async Task<ProxyDto?> GetAvailableProxyByUserId(long userId)
    {
        var userProxies = await proxyManager.GetAllProxies(userId);

        if (userProxies == default) return null;

        foreach (var proxy in userProxies)
        {
            try
            {
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                var result = await SendRequestThroughProxy(BotConstants.CHECK_PROXY_API, proxy, cts.Token);
                if (result != proxy.IP)
                {
                    continue;
                }
                return proxy;
            }
            catch
            {
                continue;
            }
        }

        return null;
    }
}

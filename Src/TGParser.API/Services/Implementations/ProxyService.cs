using System.Net;
using TGParser.API.Services.Interfaces;
using TGParser.Core.DTO;

namespace TGParser.API.Services.Implementations;

public class ProxyService : IProxyService
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
}

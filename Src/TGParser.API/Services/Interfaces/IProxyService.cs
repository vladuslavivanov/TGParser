using TGParser.Core.DTO;

namespace TGParser.API.Services.Interfaces;

public interface IProxyService
{
    Task<string> SendRequestThroughProxy(string url, ProxyDto proxy, CancellationToken? token = null);
}

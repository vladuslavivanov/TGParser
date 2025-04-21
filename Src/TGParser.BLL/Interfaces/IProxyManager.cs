using TGParser.Core.DTO;

namespace TGParser.BLL.Interfaces;

public interface IProxyManager
{
    Task<IEnumerable<ProxyDto>> GetAllProxies(long userId);
    Task AddProxy(AddProxyDto proxyDto);
    Task RemoveProxy(long userId, int proxyId);
    Task<int> GetCountProxiesByUserId(long userId);
    Task<ProxyDto?> GetProxyByShowedIdAsync(long userId, int showedId);
    Task UpdatePropertyInfoByPropertyName(long userId, int userShowedId, string propertyName, string value);
}

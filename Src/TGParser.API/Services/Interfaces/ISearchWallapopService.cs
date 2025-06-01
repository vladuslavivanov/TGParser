using TGParser.Core.DTO;
using TGParser.Core.DTO.Wallapop;

namespace TGParser.API.Services.Interfaces;

public interface ISearchWallapopService
{
    Task<Root?> Search(string userQuery, ProxyDto proxy, PresetDto? preset);
    Task<Root?> SearchNext(string nextPageParam, ProxyDto proxy);

    Task<ViewItemDto?> FilterItem(Item item, PresetDto? preset, ProxyDto proxy, CancellationToken token = default);
}

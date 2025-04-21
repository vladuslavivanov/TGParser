using Microsoft.EntityFrameworkCore;
using TGParser.BLL.Interfaces;
using TGParser.Core.DTO;
using TGParser.DAL;
using TGParser.DAL.Models;

namespace TGParser.BLL.Implementations;

public class ProxyManager(DataContext dataContext) : IProxyManager
{
    public async Task AddProxy(AddProxyDto proxyDto)
    {
        var showedId = await CalculateShowedId(proxyDto.UserId);

        var userProxy = new UserProxy()
        {
            Proxy = new()
            {
                IP = proxyDto.IP,
                Port = proxyDto.Port,
                Password = proxyDto.Password,
                UserName = proxyDto.UserName,
            },
            UserId = proxyDto.UserId,
            ShowedId = showedId
        };

        await dataContext.UserProxies.AddAsync(userProxy);

        await dataContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProxyDto>> GetAllProxies(long userId)
    {
        return await dataContext.Proxies.AsNoTracking().Include(i => i.UserProxy)
            .Where(p => p.UserProxy.UserId == userId).Select(p => new ProxyDto(
                p.UserProxy.ShowedId,
                p.IP,
                p.Port,
                p.UserName,
                p.Password,
                p.ProxyType)).ToListAsync();
    }

    public async Task RemoveProxy(long userId, int proxyId)
    {
        var proxy = await dataContext.Proxies.Include(i => i.UserProxy)
            .FirstOrDefaultAsync(p => p.UserProxy.UserId == userId && 
            p.UserProxy.ShowedId == proxyId);

        if (proxy == default) return;

        dataContext.Proxies.Remove(proxy);

        await dataContext.SaveChangesAsync();
    }

    public async Task<int> GetCountProxiesByUserId(long userId)
    {
        return await dataContext.UserProxies
            .AsNoTracking()
            .CountAsync(up => up.UserId == userId);
    }

    async Task<int> CalculateShowedId(long userId)
    {
        var allPresets = await GetAllProxies(userId);

        if (!allPresets.Any() || !allPresets.Select(s => s.ShowedId).Any(a => a == 1)) return 1;

        int minId = allPresets.Min(m => m.ShowedId);
        int maxId = allPresets.Max(m => m.ShowedId);

        int sumFromMinToMax = 0;

        for (int i = minId; i <= maxId; i++)
            sumFromMinToMax += i;

        var allShowedIdsSum = allPresets.Sum(s => s.ShowedId);

        var delta = sumFromMinToMax - allShowedIdsSum;

        return delta == 0 ? ++maxId : delta;
    }

    public async Task<ProxyDto?> GetProxyByShowedIdAsync(long userId, int showedId)
    {
        var userPreset = await dataContext.Proxies.AsNoTracking()
            .Include(i => i.UserProxy).FirstOrDefaultAsync(p => p.UserProxy.UserId == userId && p.UserProxy.ShowedId == showedId);

        if (userPreset == default) return null;

        return new ProxyDto(
            showedId,
            userPreset.IP,
            userPreset.Port,
            userPreset.UserName,
            userPreset.Password,
            userPreset.ProxyType
            );
    }

    public async Task UpdatePropertyInfoByPropertyName(long userId, int userShowedId, string propertyName, string value)
    {
        var property = typeof(Proxy).GetProperty(propertyName);
        if (property == null) return;

        var proxy = await dataContext.Proxies.Include(i => i.UserProxy)
            .FirstOrDefaultAsync(p => p.UserProxy.UserId == userId &&
            p.UserProxy.ShowedId == userShowedId);

        if (proxy == default) return;

        try
        {
            object? parsedValue = null;

            if (property.PropertyType.IsEnum)
                parsedValue = Enum.Parse(property.PropertyType, value, ignoreCase: true);
            else
                parsedValue = Convert.ChangeType(value, property.PropertyType);
            
            property.SetValue(proxy, parsedValue);

            await dataContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Тут можно логировать, если нужно
        }
    }

}

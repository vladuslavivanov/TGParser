using Microsoft.EntityFrameworkCore;
using TGParser.BLL.Interfaces;
using TGParser.Core.DTO;
using TGParser.DAL;
using TGParser.DAL.Models;

namespace TGParser.BLL.Implementations;

public class PresetManager(DataContext dataContext) : IPresetManager
{
    public async Task<IEnumerable<PresetDto>> GetAllPresetsByUserIdAsync(long userId)
    {
       var userPresets = await dataContext.Presets.AsNoTracking()
            .Include(i => i.UserPreset).Where(p => p.UserPreset.UserId == userId)
            .ToListAsync();

        return userPresets.Select(up => new PresetDto(
            userId, 
            up.UserPreset.ShowedId, 
            up.PresetName, 
            up.MinPrice, 
            up.MaxPrice, 
            up.MinDateRegisterSeller, 
            up.MaxDateRegisterSeller, 
            up.MaxNumberOfPublishBySeller, 
            up.MaxNumbersOfItemsSoldBySeller, 
            up.MaxNumberOfItemsBuysBySeller,
            up.MaxViewsByOthersWorkers,
            up.PeriodSearch));
    }

    public async Task AddPresetToUserAsync(long userId)
    {
        var showedId = await CalculateShowedId(userId);

        UserPreset newUserPreset = new() 
        { 
            Preset = new()
            {
                PresetName = $"Новый пресет {showedId}"
            },
            UserId = userId,
            IsSelected = false, 
            ShowedId = showedId
        };

        await dataContext.UserPresets.AddAsync(newUserPreset);

        await dataContext.SaveChangesAsync();
    }

    public async Task<int> GetQuantityPresetsAsync(long userId)
    {
        return await dataContext.UserPresets.CountAsync(up => up.UserId == userId);
    }

    public async Task RemovePresetByShowedIdAsync(long userId, int showedId)
    {
        var userPresents = await dataContext.Presets.Include(i => i.UserPreset)
            .FirstOrDefaultAsync(p => p.UserPreset.UserId == userId && p.UserPreset.ShowedId == showedId);

        if (userPresents == default) return;

        dataContext.Presets.Remove(userPresents);
        await dataContext.SaveChangesAsync();
    }

    public async Task<PresetDto?> GetPresetByShowedIdAsync(long userId, int showedId)
    {
        var userPreset = await dataContext.Presets.AsNoTracking()
            .Include(i => i.UserPreset).FirstOrDefaultAsync(p => p.UserPreset.UserId == userId && p.UserPreset.ShowedId == showedId);

        if (userPreset == default) return null;

        return new PresetDto(
            userId,
            userPreset.UserPreset.ShowedId,
            userPreset.PresetName,
            userPreset.MinPrice,
            userPreset.MaxPrice,
            userPreset.MinDateRegisterSeller,
            userPreset.MaxDateRegisterSeller,
            userPreset.MaxNumberOfPublishBySeller,
            userPreset.MaxNumbersOfItemsSoldBySeller,
            userPreset.MaxNumberOfItemsBuysBySeller,
            userPreset.MaxViewsByOthersWorkers,
            userPreset.PeriodSearch);
    }


    public async Task UpdatePropertyInfoByPropertyName(long userId, int userShowedId, string propertyName, string value)
    {
        var property = typeof(Preset).GetProperty(propertyName);
        if (property == null) return;

        var preset = await dataContext.Presets.Include(i => i.UserPreset)
            .FirstOrDefaultAsync(p => p.UserPreset.UserId == userId && 
            p.UserPreset.ShowedId == userShowedId);

        if (preset == default) return;

        try
        {
            property.SetValue(preset, Convert.ChangeType(value, property.PropertyType));

            await dataContext.SaveChangesAsync();
        }
        catch(Exception ex)
        {
            // Тут можно логировать, если нужно
        }
    }

    public async Task AddPreset(AddPresetDto presetDto)
    {
        var showedId = await CalculateShowedId(presetDto.UserId);

        UserPreset newUserPreset = new()
        {
            Preset = new()
            {
                PresetName = presetDto.PresetName,
                MaxPrice = presetDto.MaxPrice,
                MinPrice = presetDto.MinPrice,
                MaxDateRegisterSeller = presetDto.MaxDateRegisterSeller,  
                MinDateRegisterSeller = presetDto.MinDateRegisterSeller,
                MaxViewsByOthersWorkers = presetDto.MaxViewsByOthersWorkers,
                MaxNumbersOfItemsSoldBySeller = presetDto.MaxNumberOfItemsSoldBySeller,
                MaxNumberOfPublishBySeller = presetDto.MaxNumberOfPublishBySeller,
                MaxNumberOfItemsBuysBySeller = presetDto.MaxNumberOfItemsBuysBySeller,
                PeriodSearch = presetDto.PeriodSearch
            },
            UserId = presetDto.UserId,
            IsSelected = false,
            ShowedId = showedId
        };

        await dataContext.UserPresets.AddAsync(newUserPreset);

        await dataContext.SaveChangesAsync();
    }

    async Task<int> CalculateShowedId(long userId)
    {
        var allPresets = await GetAllPresetsByUserIdAsync(userId);

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
}

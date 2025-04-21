using Microsoft.EntityFrameworkCore;
using TGParser.BLL.Interfaces;
using TGParser.Core.DTO;
using TGParser.DAL;

namespace TGParser.BLL.Implementations;

public class UserPresetManager(DataContext dataContext) : IUserPresetManager
{
    public async Task<PresetDto?> GetSelectedPresetAsync(long userId)
    {
        var selectedPreset = await dataContext.Presets.AsNoTracking()
            .Include(i => i.UserPreset)
            .FirstOrDefaultAsync(up => up.UserPreset.UserId == userId 
            && up.UserPreset.IsSelected);

        if (selectedPreset == default) return null;

        return new PresetDto(
            userId,
            selectedPreset.UserPreset.ShowedId,
            selectedPreset.PresetName,
            selectedPreset.MinPrice,
            selectedPreset.MaxPrice,
            selectedPreset.MinDateRegisterSeller,
            selectedPreset.MaxDateRegisterSeller,
            selectedPreset.MaxNumberOfPublishBySeller,
            selectedPreset.MaxNumbersOfItemsSoldBySeller,
            selectedPreset.MaxNumberOfItemsBuysBySeller,
            selectedPreset.MaxViewsByOthersWorkers,
            selectedPreset.PeriodSearch
            );
    }

    public async Task SetDefaultPresetAsync(long userId, int presetShowedId)
    {
        await TrySetDefaultPresetAsync(userId, presetShowedId);
    }

    public async Task<bool> TrySetDefaultPresetAsync(long userId, int presetShowedId)
    {
        var userPresets = await dataContext.UserPresets
            .Where(up => up.UserId == userId).ToListAsync();

        var oldSelected = userPresets.FirstOrDefault(up => up.IsSelected);

        if (oldSelected != default) oldSelected.IsSelected = false;

        var newSelected = userPresets.FirstOrDefault(up => up.ShowedId == presetShowedId);

        if (newSelected == default) return false;

        if (newSelected != default) newSelected.IsSelected = true;

        await dataContext.SaveChangesAsync();

        return true;
    }

}

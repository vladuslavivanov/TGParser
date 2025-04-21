using TGParser.Core.DTO;

namespace TGParser.BLL.Interfaces;

public interface IPresetManager
{
    Task<IEnumerable<PresetDto>> GetAllPresetsByUserIdAsync(long userId);
    Task AddPresetToUserAsync(long userId);
    Task AddPreset(AddPresetDto presetDto);
    Task<int> GetQuantityPresetsAsync(long userId);
    Task RemovePresetByShowedIdAsync(long userId, int showedId);
    Task<PresetDto?> GetPresetByShowedIdAsync(long userId, int showedId);
    Task UpdatePropertyInfoByPropertyName(long userId, int userShowedId, string propertyName, string value);
}

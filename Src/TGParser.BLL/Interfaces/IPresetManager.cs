using TGParser.Core.DTO;
using TGParser.Core.Enums;

namespace TGParser.BLL.Interfaces;

public interface IPresetManager
{
    Task<IEnumerable<PresetDto>> GetAllPresetsByUserIdAsync(long userId);
    Task<int> AddPresetToUserAsync(long userId);
    Task AddPreset(AddPresetDto presetDto);
    Task<int> GetQuantityPresetsAsync(long userId);
    Task RemovePresetByShowedIdAsync(long userId, int showedId);
    Task<PresetDto?> GetPresetByShowedIdAsync(long userId, int showedId);
    Task UpdatePropertyInfoByPropertyName(long userId, int userShowedId, string propertyName, string value);
    Task SetSearchPeriod(long userId, int presetShowId, PeriodSearch newPeriod);
    Task SetPrice(long userId, int presetShowId, PriceType typePrice, int price);
    Task SetSellerRegistration(long userId, int presetShowId, RegistrationDataSellerType registrationType, DateTime date);
    Task SetLimitation(long userId, int presetShowId, LimitationType limitationType, int newValue);
}

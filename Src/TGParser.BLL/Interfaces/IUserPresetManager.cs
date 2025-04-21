using TGParser.Core.DTO;

namespace TGParser.BLL.Interfaces;

public interface IUserPresetManager
{
    Task<PresetDto?> GetSelectedPresetAsync(long userId);

    Task SetDefaultPresetAsync(long userId, int presetShowedId);
    Task<bool> TrySetDefaultPresetAsync(long userId, int presetShowedId);
}

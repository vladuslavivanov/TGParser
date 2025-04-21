namespace TGParser.Core.DTO;

public record SelectPresetDto(long UserId, int PresetId) : BaseDto(UserId);

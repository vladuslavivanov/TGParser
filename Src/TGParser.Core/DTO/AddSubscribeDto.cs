namespace TGParser.Core.DTO;

public record AddSubscribeDto(long UserId, int QuantityHours) : BaseDto(UserId);

using TGParser.Core.Enums;

namespace TGParser.Core.DTO;

public record AddPresetDto(
    long UserId, 
    string PresetName, 
    int MinPrice, 
    int MaxPrice, 
    DateTime MinDateRegisterSeller, 
    DateTime MaxDateRegisterSeller, 

    int MaxNumberOfPublishBySeller, 
    int MaxNumberOfItemsSoldBySeller, 
    int MaxNumberOfItemsBuysBySeller,
    
    int MaxViewsByOthersWorkers,
    PeriodSearch PeriodSearch) : BaseDto(UserId);

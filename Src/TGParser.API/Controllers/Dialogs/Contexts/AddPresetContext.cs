using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Dialogs.Contexts;

public class AddPresetContext : BaseContext
{
    public string PresetName { get; set; }
    public int MinPrice { get; set; }
    public int MaxPrice { get; set; }
    public DateTime MinDateRegisterSeller { get; set; }
    public DateTime MaxDateRegisterSaller { get; set; }

    public int MaxNumberOfPublishBySeller { get; set; }
    public int MaxNumberOfItemsSoldBySeller { get; set; }
    public int MaxNumberOfItemsBuysBySeller { get; set; }
    
    public int MaxViewsByOthersWorkers { get; set; }
    public PeriodSearch PeriodSearch { get; set; }

    public string LastRequest { get; set; }
}

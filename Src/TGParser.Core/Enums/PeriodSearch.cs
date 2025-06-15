using System.ComponentModel;

namespace TGParser.Core.Enums;

public enum PeriodSearch
{
    [Description("24 часа")]
    LAST24HOURS,
    
    [Description("7 дней")]
    LAST7DAYS,

    [Description("30 дней")]
    LAST30DAYS
}

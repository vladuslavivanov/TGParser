using System.ComponentModel;

namespace TGParser.Core.Enums;

/// <summary>
/// Период поиска: <br/>
/// 0 - последние 24 часа; <br/>
/// 1 - последние 7 дней; <br/>
/// 2 - последние 30 дней. <br/>
/// </summary>
public enum PeriodSearch
{
    /// <summary>
    /// Последние 24 часа.
    /// </summary>
    [Description("24 часа")]
    LAST24HOURS,
    
    /// <summary>
    /// Последние 7 дней.
    /// </summary>
    [Description("7 дней")]
    LAST7DAYS,

    /// <summary>
    /// Последние 30 дней.
    /// </summary>
    [Description("30 дней")]
    LAST30DAYS
}

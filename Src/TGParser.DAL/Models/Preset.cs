using TGParser.Core.Enums;

namespace TGParser.DAL.Models;

public class Preset
{
    /// <summary>
    /// Идентфиикатор пресета.
    /// </summary>
    public int PresetId { get; set; }

    /// <summary>
    /// Название пресета.
    /// </summary>
    public string PresetName { get; set; }

    /// <summary>
    /// Минимальная цена товара.
    /// </summary>
    public int MinPrice { get; set; }
    
    /// <summary>
    /// Максимальная цена товара.
    /// </summary>
    public int MaxPrice { get; set; }

    /// <summary>
    /// Максимальное количество просмотров другими воркерами.
    /// </summary>
    public int MaxViewsByOthersWorkers { get; set; }

    /// <summary>
    /// Максимальное количество просмотров карточки.
    /// </summary>
    public int MaxViewsOnSite { get; set; }

    /// <summary>
    /// Минимальная дата регистрации пользователя.
    /// </summary>
    public DateTime MinDateRegisterSeller { get; set; }
    
    /// <summary>
    /// Максимальная дата регистрации пользователя.
    /// </summary>
    public DateTime MaxDateRegisterSeller { get; set; }

    /// <summary>
    /// Максимальное количество объявлений у продавца.
    /// </summary>
    public int MaxNumberOfPublishBySeller { get; set; }

    /// <summary>
    /// Максимальное количество товаров, проданных продавцом.
    /// </summary>
    public int MaxNumbersOfItemsSoldBySeller { get; set; }

    /// <summary>
    /// Максимальное количество товаров, приобретенных продавцом.
    /// </summary>
    public int MaxNumberOfItemsBuysBySeller { get; set; }

    /// <summary>
    /// Период поиска объявлений.
    /// </summary>
    public PeriodSearch PeriodSearch { get; set; }

    #region NavProps

    public UserPreset UserPreset { get; set; }

    #endregion
}

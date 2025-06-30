using TGParser.Core.Enums;

namespace TGParser.Core.Models;

/// <summary>
/// Модель пресета.
/// </summary>
/// <param name="MinPrice">Минимальная цена объявления.</param>
/// <param name="MaxPrice">Максимальная цена объявления.</param>
/// <param name="MaxDateRegisterSeller">Максимальная дата регистрации продавца.</param>
/// <param name="MinDateRegisterSeller">Минимальная дата регистрации продавца.</param>
/// <param name="MaxViewsByOtherWorkers">Максимальное количество воркеров, которое посмотрело данное объявление.</param>
/// <param name="MaxViewsOnSite">Максимальное количество объявлений на сайте.</param>
/// <param name="MaxNumberOfPublishBySeller">Максимальное количество публикаций продавца.</param>
/// <param name="MaxNumbersOfItemsSoldBySeller">Максимальное количество товаров, которое продал продавец.</param>
/// <param name="MaxNumberOfItemsBuysBySeller">Максимальное количество товаров, которое продал продавец.</param>
/// <param name="PeriodSearch">Период поиска.</param>
public record PresetModel(
    int MinPrice, 
    int MaxPrice,
    DateTime MaxDateRegisterSeller,
    DateTime MinDateRegisterSeller,
    int MaxViewsByOtherWorkers,
    int MaxViewsOnSite,
    int MaxNumberOfPublishBySeller,
    int MaxNumbersOfItemsSoldBySeller,
    int MaxNumberOfItemsBuysBySeller,
    PeriodSearch PeriodSearch);
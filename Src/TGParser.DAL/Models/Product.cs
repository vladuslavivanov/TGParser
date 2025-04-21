namespace TGParser.DAL.Models;

public class Product
{
    /// <summary>
    /// Идентификатор товара на Wallapop.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Количество просмотров товара.
    /// </summary>
    public int CountViewed { get; set; }

    /// <summary>
    /// Дата занесения записи в БД.
    /// </summary>
    public DateTime CreateDate { get; set; }

    #region NavProps

    public List<UserViewedItems> UserViewedItems { get; set; }

    #endregion
}

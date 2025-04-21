namespace TGParser.DAL.Models;

public class UserViewedItems
{
    /// <summary>
    /// Идентификатор товара.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Время просмотра.
    /// </summary>
    public DateTime TimeView { get; set; }

    #region NavProps

    public Product Product { get; set; }
    public User User { get; set; }

    #endregion
}

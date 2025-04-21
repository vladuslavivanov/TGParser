using TGParser.Core.Enums;

namespace TGParser.DAL.Models;

public class User
{
    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Дата окончания подписки.
    /// </summary>
    public DateTime SubscriptionEndDate { get; set; }
    
    /// <summary>
    /// Идентификатор используемого пресета.
    /// </summary>
    public int SelectedPresetId { get; set; }

    /// <summary>
    /// Тип пользователя.
    /// </summary>
    public UserRole UserRole { get; set; }

    #region NavProps

    public List<UserPreset> UserPresets { get; set; }
    public List<UserViewedItems> UserViewedItems { get; set; }
    public List<UserProxy> UserProxies { get; set; }
    public List<Invoice> Invoices { get; set; }

    #endregion
}

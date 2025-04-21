namespace TGParser.DAL.Models;

public class UserPreset
{
    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    public long UserId { get; set; }
    
    /// <summary>
    /// Идентификатор пресета.
    /// </summary>
    public int PresetId { get; set; }

    /// <summary>
    /// Выбран ли данный пресет по умолчанию для данного пользователя.
    /// </summary>
    public bool IsSelected { get; set; }

    /// <summary>
    /// Показывающийся ID.
    /// </summary>
    public int ShowedId { get; set; }

    #region NavProps

    public User User { get; set; }
    public Preset Preset { get; set; }

    #endregion
}

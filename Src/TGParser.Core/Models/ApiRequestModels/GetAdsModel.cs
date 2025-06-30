namespace TGParser.Core.Models.ApiRequestModels;

/// <summary>
/// Модель запроса объявлений.
/// </summary>
public class GetAdsModel
{
    /// <summary>
    /// API ключ.
    /// </summary>
    public string ApiKey { get; init; }
    
    /// <summary>
    /// Настройки пресета.
    /// </summary>
    public PresetModel Preset { get; init; }
    
    /// <summary>
    /// Настройки прокси.
    /// </summary>
    public ProxyModel Proxy { get; init; }
}
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using TGParser.DAL.Models;

namespace TGParser.API.Controllers.Dialogs;

public static class EditingNames
{
    public static class Preset
    {
        public const string NAME = "📝 Название пресета";
        public const string MAX_PRICE = "📈 Максимальная цена";
        public const string MIN_PRICE = "📉 Минимальная цена";

        public const string MAX_DATA_REGISTER_SELLER = "🔼 Дата макс. рег-ии продавца";
        public const string MIN_DATA_REGISTER_SELLER = "🔽 Дата мин. рег-ии продавца";

        public const string MAX_VIEWS_BY_OTHER_WORKERS = "👀 Макс. кол-во просмотров товара другими воркерами";
        public const string MAX_NUMBER_OF_ITEMS_SOLD_BY_SELLER = "🛒 Макс. кол-во проданных товаров продавцом";
        public const string MAX_NUNMBER_OF_ITEMS_BUYS_BY_SELLER = "🛍️ Макс. кол-во приобретенный товаров продавцом";
        public const string MAX_NUMBER_OF_PUBLISH_BY_SELLER = "🛍️ Максимальное количество объявлений у продавца";

        public const string SEARCH_PERIOD_IN_DAYS = "🕰️Период поиска объявлений";

        public static bool IsPresetValue(string value)
        {
            return typeof(Preset)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Any(f => f.GetRawConstantValue()?.ToString() == value);
        }
    }

    public static class Proxy
    {
        [Editable(true)]
        public const string IP = "🌐 IP прокси сервера";
        
        [Editable(true)]
        public const string PORT = "🔌 Порт прокси сервера";
        
        [Editable(true)]
        public const string USER = "👤 Логин от прокси сервера";
        
        [Editable(true)]
        public const string PASSWORD = "🔒 Пароль от прокси сервера";
        
        public const string PROXY_TYPE = "🧭 Тип прокси сервера";

        public static bool IsPresetValue(string value)
        {
            return typeof(Proxy)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Any(f => f.GetRawConstantValue()?.ToString() == value);
        }
    }

    public static class Search
    {
        public const string WITHOUT_KEYWORDS = "🔍 Найти всё самое новое";

        public const string NEXT_PAGE = "➡️ Следующая страница";
    }

    public const string LEAVE = "⬅️ Назад";
}

public static class PresetMapper
{
    public static Dictionary<string, string> EditingNamePropertyInPresetTable = new()
    {
        {EditingNames.Preset.NAME, nameof(Preset.PresetName) },

        {EditingNames.Preset.MAX_PRICE, nameof(Preset.MaxPrice) },
        {EditingNames.Preset.MIN_PRICE, nameof(Preset.MinPrice) },

        {EditingNames.Preset.MAX_DATA_REGISTER_SELLER, nameof(Preset.MaxDateRegisterSeller) },
        {EditingNames.Preset.MIN_DATA_REGISTER_SELLER, nameof(Preset.MinDateRegisterSeller) },

        {EditingNames.Preset.MAX_VIEWS_BY_OTHER_WORKERS, nameof(Preset.MaxViewsByOthersWorkers) },
        {EditingNames.Preset.MAX_NUMBER_OF_ITEMS_SOLD_BY_SELLER, nameof(Preset.MaxNumbersOfItemsSoldBySeller) },
        {EditingNames.Preset.MAX_NUNMBER_OF_ITEMS_BUYS_BY_SELLER, nameof(Preset.MaxNumberOfItemsBuysBySeller) },
        {EditingNames.Preset.MAX_NUMBER_OF_PUBLISH_BY_SELLER, nameof(Preset.MaxNumberOfPublishBySeller) },

        {EditingNames.Preset.SEARCH_PERIOD_IN_DAYS, nameof(Preset.PeriodSearch) },
    };

    public static Dictionary<string, string> EditingNamePropertyInProxyTable = new()
    {
        {EditingNames.Proxy.IP, nameof(Proxy.IP) },
        {EditingNames.Proxy.PORT, nameof(Proxy.Port) },
        {EditingNames.Proxy.USER, nameof(Proxy.UserName) },
        {EditingNames.Proxy.PASSWORD, nameof(Proxy.Password) },
        {EditingNames.Proxy.PROXY_TYPE, nameof(Proxy.ProxyType) },
    };
}

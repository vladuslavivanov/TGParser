using Newtonsoft.Json;
using System.Text;
using TGParser.API.Converters;
using TGParser.API.Services.Interfaces;
using TGParser.Core.DTO;
using TGParser.Core.DTO.Wallapop;

namespace TGParser.API.Services.Implementations;

public class SearchWallapopService(IProxyService proxyService) : ISearchWallapopService
{
    public async Task<Root?> Search(string userQuery, ProxyDto proxy, PresetDto? preset)
    {
        StringBuilder stringQuery = new("https://api.wallapop.com/api/v3/search?");

        stringQuery.Append($"longitude=-3.69196");
        stringQuery.Append($"&latitude=40.41956");

        if (userQuery != string.Empty)
            stringQuery.Append($"&keywords={userQuery}");

        
        stringQuery.Append($"&order_by=newest");

        Root? root = default;

        if (preset != default)
        {
            stringQuery.Append("&source=side_bar_filters");

            stringQuery.Append($"&min_sale_price={preset.MinPrice}");
            stringQuery.Append($"&max_sale_price={preset.MaxPrice}");

            var period = string.Empty;

            switch (preset.PeriodSearch)
            {
                case Core.Enums.PeriodSearch.LAST24HOURS:
                    period = "today";
                    break;
                case Core.Enums.PeriodSearch.LAST7DAYS:
                    period = "lastWeek";
                    break;
                case Core.Enums.PeriodSearch.LAST30DAYS:
                    period = "lastMonth";
                    break;
                default:
                    break;
            }
            stringQuery.Append($"&time_filter={period}");
        }
        else
        {
            stringQuery.Append($"&source=search_box");
        }

        var json = await proxyService.SendRequestThroughProxy(stringQuery.ToString(), proxy);

        var settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new TrimEmptyLinesConverter() }
        };

        try
        {
            root = JsonConvert.DeserializeObject<Root>(json, settings);
        }
        catch (Exception ex)
        {
            return null;
        }

        return root;
    }

    public async Task<Root?> SearchNext(string nextPageParam, ProxyDto proxy)
    {
        StringBuilder stringQuery = new("https://api.wallapop.com/api/v3/search?");

        stringQuery.Append($"&next_page={nextPageParam}");

        var json = await proxyService.SendRequestThroughProxy(stringQuery.ToString(), proxy);

        Root? root = default;

        var settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new TrimEmptyLinesConverter() }
        };

        try
        {
            root = JsonConvert.DeserializeObject<Root>(json, settings);
        }
        catch (Exception ex)
        {
            return null;
        }

        return root;
    }

    public async Task<ViewItemDto?> FilterItem(Item item, PresetDto? preset, ProxyDto proxy, CancellationToken token = default)
    {
        var userInfo = await GetUserInfo(item.UserId, proxy, token);

        // Сколько у пользователя опубликовано в данный момент объявлений.
        var publish = userInfo.Stats.Counters.FirstOrDefault(c => c.Type == "publish")?.Value;

        // Сколько пользователь продал.
        var sells = userInfo.Stats.Counters.FirstOrDefault(c => c.Type == "sells")?.Value;

        // Сколько пользователь купил.
        var buys = userInfo.Stats.Counters.FirstOrDefault(c => c.Type == "buys")?.Value;

        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(userInfo.RegisterDate);
        DateTime registerDate = dateTimeOffset.DateTime;

        dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(item.CreatedAt);
        var createdDate = dateTimeOffset.DateTime;

        dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(item.ModifiedAt);
        var modifiedDate = dateTimeOffset.DateTime;

        var dateTimeNow = DateTime.UtcNow;

        if (preset != default)
        {
            bool isInRangeDateRegister =
                registerDate >= preset.MinDateRegisterSeller
                && registerDate <= preset.MaxDateRegisterSaller
                && publish <= preset.MaxNumberOfPublishBySeller
                && sells <= preset.MaxNumberOfItemsSoldBySeller
                && buys <= preset.MaxNumberOfItemsBuysBySeller;

            if (!isInRangeDateRegister)
                return null;

            switch (preset.PeriodSearch)
            {
                case Core.Enums.PeriodSearch.LAST24HOURS:
                    if (createdDate.AddDays(1) < dateTimeNow) 
                        return null;
                    break;
                case Core.Enums.PeriodSearch.LAST7DAYS:
                    if (createdDate.AddDays(7) < dateTimeNow)
                        return null;
                    break;
                case Core.Enums.PeriodSearch.LAST30DAYS:
                    if (createdDate.AddDays(30) < dateTimeNow)
                        return null;
                    break;
                default:
                    break;
            }

        }

        item.LinkToUser = userInfo.UrlShare;
        item.LinkToChat = "https://es.wallapop.com/app/chat?itemId=" + item.Id;

        return new(item.Id,
                "https://es.wallapop.com/item/" + item.WebSlug,
                item.Title.Trim(),
                item.Description.Trim(),
                item.Price.Amount.ToString() + " "+ item.Price.Currency,
                item.LinkToUser,
                item.LinkToChat.ToString(),
                createdDate,
                modifiedDate);
    }

    async Task<UserProfileV3> GetUserInfo(string userId, ProxyDto proxy, CancellationToken token = default)
    {
        var userQuery = $"https://api.wallapop.com/api/v3/users/{userId}/";
        var userQueryStats = userQuery + "stats";

        var jsonUserQuery = await proxyService.SendRequestThroughProxy(userQuery, proxy, token);
        var jsonUserQueryStats = await proxyService.SendRequestThroughProxy(userQueryStats, proxy, token);

        UserProfileV3? userProfile = default;
        UserProfileV3Stats? userProfileStats = default;

        try
        {
            userProfile = JsonConvert.DeserializeObject<UserProfileV3>(jsonUserQuery);
            userProfileStats = JsonConvert.DeserializeObject<UserProfileV3Stats>(jsonUserQueryStats);
        }
        catch (Exception ex)
        {
            return null;
        }

        userProfile!.Stats = userProfileStats!;

        return userProfile;
    }

}

using Newtonsoft.Json;

namespace TGParser.Core.DTO.Wallapop;

public record UserProfileV3(
    string Id,
    [property: JsonProperty("register_date")] long RegisterDate,
    [property: JsonProperty("web_slug")] string WebSlug,
    [property: JsonProperty("url_share")] string UrlShare
)
{
    public UserProfileV3Stats Stats { get; set; }
};

public record UserProfileV3Stats(
    List<Counter> Counters
);

public record Counter(
    string Type,
    int Value
);
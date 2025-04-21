using Newtonsoft.Json;

namespace TGParser.Core.DTO.Wallapop;

public record Root(
    Data Data,
    Meta Meta
);

public record Data(
    Section Section
);

public record Section(
    string Type,
    Payload Payload
);

public record Payload(
    string Order,
    string Title,
    List<Item> Items
);

public record Item(
    string Id,
    [property: JsonProperty("user_id")] string UserId,
    string Title,
    string Description,
    int CategoryId,
    Price Price,
    List<Image> Images,
    Reserved Reserved,
    [property: JsonProperty("web_slug")] string WebSlug,
    [property: JsonProperty("created_at")] long CreatedAt,
    [property: JsonProperty("modified_at")] long ModifiedAt
    )
{
    public string LinkToUser { get; set; }
    public string LinkToChat { get; set; }
};

public record Price(
    double Amount,
    string Currency
);

public record Image(
    string Id,
    string AverageColor,
    Urls Urls
);

public record Urls(
    string Small,
    string Medium,
    string Big
);

public record Reserved(
    bool Flag
);

public record Meta(
    [property: JsonProperty("next_page")] string NextPage
);

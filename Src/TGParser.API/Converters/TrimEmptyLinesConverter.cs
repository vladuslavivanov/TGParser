using Newtonsoft.Json;

namespace TGParser.API.Converters;

public class TrimEmptyLinesConverter : JsonConverter
{
    public override bool CanConvert(Type objectType) => objectType == typeof(string);

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.Value == null) return "";
        string value = (string)reader.Value;
        value = value.Trim();
        return string.Join(
            Environment.NewLine,
            value.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                 .Where(line => !string.IsNullOrWhiteSpace(line))
        );
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteValue(value);
    }
}

using System.Text;

namespace TGParser.Core.DTO.Wallapop;

public record ViewItemDto(
    string itemId,
    string LinkToProduct, 
    string Title, 
    string Description, 
    string Price,
    string LinkToSeller,
    string LinkToChat,
    DateTime CreatedAt,
    DateTime ModifiedAt)
{
    public override string ToString()
    {
        StringBuilder result = new();

        result.AppendLine($"📝 <b>Название</b>: {Title}");
        result.AppendLine();
        result.AppendLine($"📜 <b>Описание</b>: {(Description?.Length > 150 ? Description.Substring(0, 150) + "..." : Description)}");
        result.AppendLine();
        result.AppendLine($"💰 <b>Цена</b>: {Price}");
        result.AppendLine();
        result.AppendLine($"⏱️ <b>Дата размещения</b>: {CreatedAt.AddHours(3).ToString()} (МСК)");
        result.AppendLine($"⏱️ <b>Дата редактирования</b>: {ModifiedAt.AddHours(3).ToString()} (МСК)");
        result.AppendLine();
        result.AppendLine($"🛒 <a href=\"{LinkToProduct}\">Ссылка на продукт</a>");
        result.AppendLine($"👤 <a href=\"{LinkToSeller}\">Ссылка на продавца</a>");
        result.AppendLine($"💬 <a href=\"{LinkToChat}\">Ссылка на чат</a>");

        return result.ToString();
    }
}
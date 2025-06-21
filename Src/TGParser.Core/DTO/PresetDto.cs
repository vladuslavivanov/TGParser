using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using TGParser.Core.Enums;

namespace TGParser.Core.DTO;

public record PresetDto(
    long UserId,
    int ShowedId,
    bool IsSelected,
    string PresetName,
    int MinPrice,
    int MaxPrice,
    DateTime MinDateRegisterSeller,
    DateTime MaxDateRegisterSeller,
    int MaxNumberOfPublishBySeller,
    int MaxNumberOfItemsSoldBySeller,
    int MaxNumberOfItemsBuysBySeller,
    int MaxViewsByOthersWorkers,
    PeriodSearch PeriodSearch) : BaseDto(UserId)
{
    public override string ToString()
    {
        var enumType = typeof(PeriodSearch);

        var memberInfo = enumType
                .GetMember(PeriodSearch.ToString());

        var enumValueMemberInfo = memberInfo
            .FirstOrDefault(m => m.DeclaringType == enumType);

        var valueAttributes = enumValueMemberInfo!
                .GetCustomAttribute<DescriptionAttribute>(false);

        var periodSearch = valueAttributes!.Description;

        StringBuilder sb = new();

        sb.AppendLine("👤 Информация о пресете:");
        sb.AppendLine($"№ Пресета - {ShowedId}");
        //sb.AppendLine($"Название пресета - {PresetName}");
        sb.AppendLine($"Период поиска объявлений - {periodSearch}");
        
        sb.AppendLine();

        sb.AppendLine("💰 Параметры цен:");
        sb.AppendLine($"Мин. цена - {MinPrice} €");
        sb.AppendLine($"Макс. цена - {MaxPrice} €");

        sb.AppendLine();

        sb.AppendLine("📅 Период регистрации продавца:");
        sb.AppendLine($"Дата мин. регистрации продавца - {MinDateRegisterSeller:dd.MM.yyyy}");
        sb.AppendLine($"Дата макс. регистрации продавца - {MaxDateRegisterSeller:dd.MM.yyyy}");

        sb.AppendLine();

        sb.AppendLine("📦 Ограничения по товарам и спискам продавца:");
        sb.AppendLine($"Макс. кол-во просмотров товара другими воркерами - {MaxViewsByOthersWorkers}");
        sb.AppendLine($"Макс. кол-во открытых объявлений у продавца - {MaxNumberOfPublishBySeller}");
        sb.AppendLine($"Макс. кол-во проданных товаров продавцом - {MaxNumberOfItemsSoldBySeller}");
        sb.AppendLine($"Макс. кол-во купленных товаров продавцом - {MaxNumberOfItemsBuysBySeller}");

        return sb.ToString();
    }
}

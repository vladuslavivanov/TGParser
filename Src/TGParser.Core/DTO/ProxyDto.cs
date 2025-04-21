using System.Text;
using TGParser.Core.Enums;

namespace TGParser.Core.DTO;

public record ProxyDto(
    int ShowedId, 
    string IP, 
    int Port, 
    string UserName, 
    string Password, 
    ProxyType ProxyType)
{
    public override string ToString()
    {
        StringBuilder sb = new();

        sb.AppendLine($"🔢 № Прокси - {ShowedId}");
        sb.AppendLine();
        sb.AppendLine($"🌐 IP - {IP}");
        sb.AppendLine($"📍 Порт - {Port}");
        sb.AppendLine();
        sb.AppendLine($"👤 Имя пользователя - {UserName}");
        sb.AppendLine($"🔑 Пароль - {Password}");
        sb.AppendLine();
        sb.AppendLine($"⚙️ Тип прокси - {ProxyType.ToString()}");

        return sb.ToString();
    }
}

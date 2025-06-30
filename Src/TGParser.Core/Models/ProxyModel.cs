namespace TGParser.Core.Models;

/// <summary>
/// Модель прокси сервера.
/// </summary>
/// <param name="IP">IP прокси сервера.</param>
/// <param name="Port">Порт прокси сервера.</param>
/// <param name="User">Имя пользователя прокси сервера.</param>
/// <param name="Password">Пароль прокси сервера.</param>
public record ProxyModel(string IP, int Port, string User, string Password);
using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot;
using Telegram.Bot.Types;
using TGParser.API.Services.Interfaces;
using TGParser.API.Utils;
using TGParser.BLL.Interfaces;
using TGParser.Configuration;
using TGParser.Core.DTO;

namespace TGParser.API.Services.Implementations;

public class TelegramUserService(IUserManager userManager,
    ITelegramBotClient client) : ITelegramUserService
{
    static readonly int TrialDays = ConfigurationStorage.GetTrialDays();
    static readonly string Channel = ConfigurationStorage.GetChannelName();

    static MemoryCache MemoryCache = new MemoryCache(new MemoryCacheOptions() 
    { 
        ExpirationScanFrequency = TimeSpan.FromMinutes(10),
    });

    public async Task AddUserIfNotExists(Message message)
    {
        var userId = message.From!.Id;

        var metaData = GetOrCreateMetaData(userId);

        if (metaData.IsRegistered)
        {
            return;
        }
        
        var userExists = await userManager.CheckUserExists(userId);

        if (userExists)
        {
            metaData.IsRegistered = true;
            return;
        }

        await userManager.AddIfNotExistsAsync(userId);

        if (TrialDays > 0)
        {
            await userManager.AddSubscription(userId, TrialDays);
        }

        await client.SendMessage(
            message.Chat.Id,
            $"""
            Приветствуем тебя! 👋
            Рады видеть тебя среди наших пользователей!
            Мы ценим каждого клиента, поэтому дарим тебе пробный период на {TrialDays} {RussianPluralizationUtils.GetDayWord(TrialDays)} 🕒 
            Так ты сможешь познакомиться с нашим парсером 🔍
            Если возникнут вопросы — пиши в поддержку! 💬 
            Мы всегда рядом! 🤝
            """);
    }

    public async Task<bool> IsUserSubscribed(Message message)
    {
        var userId = message.From!.Id;

        var metaData = GetOrCreateMetaData(userId);

        if (metaData.IsSubcribe)
        {
            return true;
        }

        var chatMember = await client.GetChatMember(new("@" + Channel), userId);

        if (chatMember == default)
        {
            throw new NullReferenceException("Телеграмм не вернул ответ о подписке пользователя на канал.");
        }

        if (chatMember.Status == Telegram.Bot.Types.Enums.ChatMemberStatus.Administrator ||
            chatMember.Status == Telegram.Bot.Types.Enums.ChatMemberStatus.Creator ||
            chatMember.Status == Telegram.Bot.Types.Enums.ChatMemberStatus.Member)
        {
            metaData.IsSubcribe = true;
            return true;
        }

        await client.SendMessage(message.Chat.Id,
            "Привет! 👋 Для того, чтобы пользоваться ботом, " +
            "необходимо быть подписанным на наш официальный Telegram-канал. 📢\n" +
            $"<a href=\"https://t.me/{Channel}\">Подписаться на канал</a>",
            Telegram.Bot.Types.Enums.ParseMode.Html);

        return false;
    }

    UserMetaData GetOrCreateMetaData(long userId)
    {
        MemoryCache.TryGetValue(userId, out var value);

        var metaData = (UserMetaData)value! ?? null;

        if (metaData == default)
        {
            metaData = MemoryCache.Set(userId, new UserMetaData(), new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(1),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
            });
        }

        return metaData;
    }

}

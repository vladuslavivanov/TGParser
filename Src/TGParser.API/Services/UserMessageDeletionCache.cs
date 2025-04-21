using System.Collections.Concurrent;
using Telegram.Bot;

namespace TGParser.API.Services;

public static class UserMessageDeletionCacheService
{
    private static readonly ConcurrentDictionary<long, List<int>> messagesToDelete = new();

    public static void AddMessage(long chatId, int messageId)
    {
        messagesToDelete.TryGetValue(chatId, out var messages);
        if (messages != default)
        {
            messages.Add(messageId);
            return;
        }

        messagesToDelete.TryAdd(chatId, new() { messageId });
    }

    public async static Task ClearMessages(ITelegramBotClient client, long chatId)
    {
        messagesToDelete.TryRemove(chatId, out var messages);
        if (messages == default) return;

        foreach (var message in messages)
        {
            try
            {
                await client.DeleteMessage(chatId, message);
            }
            catch(Exception ex) { }
        }
    }
}

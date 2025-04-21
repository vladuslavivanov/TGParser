using Telegram.Bot.Types;
using Telegram.Bot;
using TGParser.Services.Implementations;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;

namespace TGParser.Services.Interfaces;

public class MessageHandler(ILogger<MessageHandler> logger, ITelegramBotClient client) : IMessageHandler
{
    public async Task HandleMessageAsync(Update update)
    {
        if (update.Message!.Text is not string text) return;

        var chatId = update.Message.Chat.Id;
        logger.LogInformation("Message from {ChatId}: {Text}", chatId, text);

        var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[] // первая строка кнопок
                {
                    InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Действие 1", "action_1"),
                    InlineKeyboardButton.WithSwitchInlineQuery("Действие 2", "action_2")
                },
                new[] // вторая строка кнопок
                {
                    InlineKeyboardButton.WithCallbackData("Действие 3", "action_3"),
                    InlineKeyboardButton.WithCopyText("Действие 4", "action_4")
                }
            });

        //await client.SendMessage(chatId, $"Ты написал: {text}", replyMarkup: inlineKeyboard);
        await client.SendMessage(chatId, $"Ты написал: {text}", replyMarkup: new ReplyKeyboardRemove());
    }
}

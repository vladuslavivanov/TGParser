using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.Commands.Interfaces;
using TGParser.API.Utils;
using TGParser.BLL.Interfaces;

namespace TGParser.API.Controllers.Commands.Implementations;

public class ProfileCommand(ITelegramBotClient client,
    IUserManager userManager, IUserPresetManager userPresetManager) : BaseCommand, ICommand
{
    public string Name => CommandNames.PROFILE;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var keyboard = new ReplyKeyboardMarkup(
        [
            [CommandNames.PRESETS, CommandNames.PROXIES],
            [CommandNames.SET_DEFAULT_PRESET],
            // [CommandNames.BUY_DAYS],
            [CommandNames.HOME],
        ])
        {
            ResizeKeyboard = true,
        };

        var date = await userManager.GetSubscriptionEndDateAsync(UserId);

        var defaultPreset = await userPresetManager.GetSelectedPresetAsync(UserId);

        var defaultPresetSB = new StringBuilder();
        defaultPresetSB.AppendLine(defaultPreset == default ?
            "📌 Пресет для поиска объявлений не выбран" :
            "📌 Пресет для поиска объявлений\n\n" +
            defaultPreset.ToString());

        var subscribeSB = new StringBuilder();

        subscribeSB.AppendLine(GetQuantitySubscribe((DateTime)date));

        await client.SendMessage(
            chatId: ChatId,
            text: defaultPresetSB.ToString()
            );

        await client.SendMessage(
             chatId: ChatId,
             text: subscribeSB.ToString(),
             replyMarkup: keyboard
             );
    }

    string GetQuantitySubscribe(DateTime endDateSubscribe)
    {
        var now = DateTime.UtcNow;
        var subscriptionEndsAt = endDateSubscribe;

        var remaining = subscriptionEndsAt - now;
        if (remaining <= TimeSpan.Zero)
        {
            return "⛔️ Подписка не активна";
        }

        int days = remaining.Days;
        int hours = remaining.Hours;
        int minutes = remaining.Minutes;

        StringBuilder message = new();

        message.Append("⏳ Подписка активна. Осталось: ");

        if (days != default)
        {
            message.Append($"{days} {RussianPluralizationUtils.GetDayWord(days)} ");
        }

        if (hours != default)
        {
            message.Append($"{hours} {RussianPluralizationUtils.GetHourWord(hours)} ");
        }

        if (minutes != default)
        {
            message.Append($"{minutes} {RussianPluralizationUtils.GetMinuteWord(minutes)}");
        }

        return message.ToString();
    }
}

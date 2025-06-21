using System.ComponentModel;
using System.Reflection;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.Core.Enums;
using TGParser.Core.Enums.Presets;

namespace TGParser.API.Controllers.CallbackQueries;

public static class CallbackQueryHelper
{
    public static InlineKeyboardMarkup GetParseLimitInlineKeyboardMarkup(long userId) =>
        new InlineKeyboardMarkup(
        [
            [
                    InlineKeyboardButton.WithCallbackData("20", $"{CallbackQueryNames.PARSE_LIMIT}_20_{userId}"),
                    InlineKeyboardButton.WithCallbackData("40", $"{CallbackQueryNames.PARSE_LIMIT}_40_{userId}"),
                    InlineKeyboardButton.WithCallbackData("80", $"{CallbackQueryNames.PARSE_LIMIT}_80_{userId}"),
            ]
        ]);

    public static InlineKeyboardMarkup SelectActionPreParse(string userId, int quantityAdv) =>
        new InlineKeyboardMarkup(
        [
            [
                InlineKeyboardButton.WithCallbackData("📰 Найти все новые объявления", $"{CallbackQueryNames.SEARCH_ADV}_{quantityAdv}_{userId}_{"-"}"),
            ],
            [
                InlineKeyboardButton.WithCallbackData("🖊 Написать свой запрос [В разработке]", $"{CallbackQueryNames.SEARCH_MY_REQUEST}_{quantityAdv}_{userId}"),
            ]
        ]);

    public static InlineKeyboardMarkup ActiveCancellationToken(string userId, string source) =>
        new InlineKeyboardMarkup(
        [
            [
                InlineKeyboardButton.WithCallbackData("❌ Отмена", $"{CallbackQueryNames.ACTIVE_CANCELATION_TOKEN}_{userId}_{source}"),
            ]
        ]);

    public static InlineKeyboardMarkup ConfigurePeriodSearchKeyboard(int presetId)
    {
        var values = Enum.GetValues<PeriodSearch>();
        var enumType = typeof(PeriodSearch);

        var inlineKeyboardMarkup = new InlineKeyboardMarkup();

        foreach (var value in values)
        {
            var memberInfo = enumType
                .GetMember(value.ToString());

            var enumValueMemberInfo = memberInfo
                .FirstOrDefault(m => m.DeclaringType == enumType);

            var valueAttributes = enumValueMemberInfo!
                    .GetCustomAttribute<DescriptionAttribute>(false);

            var description = valueAttributes!.Description;

            inlineKeyboardMarkup.AddButton(InlineKeyboardButton.WithCallbackData(description, $"{CallbackQueryNames.SET_SEARCH_PERIOD_PRESET}_{presetId}_{SetSearchPeriodStep.SetSearchPeriod}_{(int)value}"));
        }

        inlineKeyboardMarkup.AddNewRow(InlineKeyboardButton.WithCallbackData("◀️ Назад", $"{CallbackQueryNames.EDIT_PRESET}_{presetId}"));     

        return inlineKeyboardMarkup;
    }
}

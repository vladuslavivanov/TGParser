using Telegram.Bot.Types.ReplyMarkups;

namespace TGParser.API.Controllers.CallbackQueries;

public static class Static
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
}

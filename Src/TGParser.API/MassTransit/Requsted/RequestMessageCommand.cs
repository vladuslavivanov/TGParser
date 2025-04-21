using Telegram.Bot.Types;

namespace TGParser.API.MassTransit.Requsted;

public record RequestMessageCommand(Update Update, string? CommandName = null);

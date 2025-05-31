using MassTransit;
using TGParser.API.Controllers.Messages;
using TGParser.API.MassTransit.Requsted;
namespace TGParser.API.MassTransit.Consumers;

public class MessageCommandConsumer(CommandExecutor commandExecutor) : IConsumer<RequestMessageCommand>
{
    public async Task Consume(ConsumeContext<RequestMessageCommand> context)
    {
        if (context.Message.CommandName == default)
            await commandExecutor.Execute(context.Message.Update);
        else
            await commandExecutor.DispatchAsync(context.Message.CommandName, context.Message.Update);
    }
}

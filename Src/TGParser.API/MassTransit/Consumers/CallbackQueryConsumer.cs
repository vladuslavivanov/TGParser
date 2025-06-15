using MassTransit;
using TGParser.API.Controllers.CallbackQueries;
using TGParser.API.MassTransit.Requsted;

namespace TGParser.API.MassTransit.Consumers;

public class CallbackQueryConsumer(CallbackQueryExecutor executor) : IConsumer<RequestBeginCallbackQuery>
{
    public async Task Consume(ConsumeContext<RequestBeginCallbackQuery> context)
    {
        await executor.Execute(context.Message.Update);
    }
}

using MassTransit;
using TGParser.API.Controllers.Dialogs.Interfaces;
using TGParser.API.MassTransit.Requsted;

namespace TGParser.API.MassTransit.Consumers;

public class DialogConsumer(IDialogExecutor dialogExecutor) : IConsumer<RequestDialogCommand>
{
    public async Task Consume(ConsumeContext<RequestDialogCommand> context)
    {
        await dialogExecutor.ExecuteDialog(context.Message.Message);
    }
}

using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Dialogs;

public class BaseContext
{
    public DialogState DialogState { get; set; }

    public bool InProgress { get; set; } = false;
}

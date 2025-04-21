using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Dialogs;

public class DialogContext
{
    public DialogState DialogState { get; set; }
    
    public int IdEntity { get; set; }
    public string ChoosedField { get; set; }
    public string NewValue { get; set; }

    public string Link { get; set; }
}

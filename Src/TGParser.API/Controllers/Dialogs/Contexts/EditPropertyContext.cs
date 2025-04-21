namespace TGParser.API.Controllers.Dialogs.Contexts;

public class EditPropertyContext : BaseContext
{
    public int IdEntity { get; set; }
    public string ChoosedField { get; set; }
    public string NewValue { get; set; }
}

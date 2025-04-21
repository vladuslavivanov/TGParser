namespace TGParser.API.Controllers.Dialogs.Contexts;

public class BuyDaysContext : BaseContext
{
    public int Price { get; set; }
    public int QuantityDays { get; set; }
    public string LinkToPay { get; set; }
}

namespace TGParser.API.Controllers.Dialogs.Contexts;

public class AddProxyContext : BaseContext
{
    public string IP { get; set; }
    public int Port { get; set; }

    public string UserName { get; set; }
    public string Password { get; set; }

    public string LastRequest { get; set; }
}

using TGParser.Core.Enums;

namespace TGParser.DAL.Models;

public class Proxy
{
    public int ProxyId { get; set; }
    public string IP { get; set; }
    public int Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public ProxyType ProxyType { get; set; }

    #region Nav Props

    public UserProxy UserProxy { get; set; }

    #endregion
}

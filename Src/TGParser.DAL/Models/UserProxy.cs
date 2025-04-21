namespace TGParser.DAL.Models;

public class UserProxy
{
    public long UserId { get; set; }
    public int ProxyId { get; set; }

    public int ShowedId { get; set; }

    #region Nav Props

    public User User { get; set; }
    public Proxy Proxy { get; set; }

    #endregion

}

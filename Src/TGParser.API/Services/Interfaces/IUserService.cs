namespace TGParser.API.Services.Interfaces;

public interface IUserService
{
    Task<bool> IsUserSubscribed(long userId);
}

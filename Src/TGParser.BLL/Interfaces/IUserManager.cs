namespace TGParser.BLL.Interfaces;

public interface IUserManager
{
    Task<DateTime?> GetSubscriptionEndDateAsync(long userId);

    Task AddIfNotExistsAsync(long userId);

    Task<bool> CheckUserExists(long userId);

    Task AddSubscription(long userId, int days);
}

using Microsoft.EntityFrameworkCore;
using TGParser.BLL.Interfaces;
using TGParser.DAL;

namespace TGParser.BLL.Implementations;

public class UserManager(DataContext dataContext) : IUserManager
{
    public async Task<DateTime?> GetSubscriptionEndDateAsync(long userId)
    {
        var user = await dataContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            return null;

        return user?.SubscriptionEndDate;
    }

    public async Task AddIfNotExistsAsync(long userId)
    {
        var user = await dataContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user != default) return;

        user = new DAL.Models.User()
        {
            UserId = userId,
            SubscriptionEndDate = DateTime.UtcNow
        };

        await dataContext.Users.AddAsync(user);

        await dataContext.SaveChangesAsync();
    }

    public async Task AddSubscription(long userId, int days)
    {
        var user = await dataContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == default) return;

        user.SubscriptionEndDate = user.SubscriptionEndDate.AddDays(days);

        await dataContext.SaveChangesAsync();
    }


    public async Task<bool> CheckUserExists(long userId)
    {
        return await dataContext.Users.AnyAsync(u => u.UserId == userId);
    }
}

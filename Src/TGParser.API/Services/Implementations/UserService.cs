using Microsoft.EntityFrameworkCore;
using TGParser.API.Services.Interfaces;
using TGParser.DAL;

namespace TGParser.API.Services.Implementations;

public class UserService(DataContext dataContext) : IUserService
{
    public async Task<bool> IsUserSubscribed(long userId)
    {
        var subscriptionEndDate = (await dataContext.Users.FirstAsync(u => u.UserId == userId))
            .SubscriptionEndDate;

        var remaining = subscriptionEndDate - DateTime.Now;
        return remaining > TimeSpan.Zero;
    }
}

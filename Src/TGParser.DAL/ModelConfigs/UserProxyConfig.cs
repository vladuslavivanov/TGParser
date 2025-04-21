using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGParser.DAL.Models;

namespace TGParser.DAL.ModelConfigs;

public class UserProxyConfig : IEntityTypeConfiguration<UserProxy>
{
    public void Configure(EntityTypeBuilder<UserProxy> builder)
    {
        builder.HasKey(k => new { k.ProxyId, k.UserId });

        builder.HasOne(o => o.Proxy)
            .WithOne(o => o.UserProxy);

        builder.HasOne(o => o.User)
            .WithMany(m => m.UserProxies);
    }
}

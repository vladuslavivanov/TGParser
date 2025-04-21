using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGParser.DAL.Models;

namespace TGParser.DAL.ModelConfigs;

public class UserViewedItemsConfig : IEntityTypeConfiguration<UserViewedItems>
{
    public void Configure(EntityTypeBuilder<UserViewedItems> builder)
    {
        builder.HasKey(k => new { k.UserId, k.ProductId });

        builder.HasOne(o => o.User)
            .WithMany(m => m.UserViewedItems);

        builder.HasOne(o => o.Product)
            .WithMany(m => m.UserViewedItems);
    }
}

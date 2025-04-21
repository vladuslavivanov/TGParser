using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGParser.Core.Enums;
using TGParser.DAL.Models;

namespace TGParser.DAL.ModelConfigs;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(k => k.UserId);

        builder.Property(p => p.UserRole)
            .HasDefaultValue(UserRole.User);
    }
}

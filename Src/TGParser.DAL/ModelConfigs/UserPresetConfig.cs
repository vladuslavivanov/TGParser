using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGParser.DAL.Models;

namespace TGParser.DAL.ModelConfigs;

public class UserPresetConfig : IEntityTypeConfiguration<UserPreset>
{
    public void Configure(EntityTypeBuilder<UserPreset> builder)
    {
        builder.HasKey(k => new { k.PresetId, k.UserId });

        builder.HasOne(o => o.User)
            .WithMany(m => m.UserPresets);

        builder.HasOne(o => o.Preset)
            .WithOne(o => o.UserPreset);
    }
}

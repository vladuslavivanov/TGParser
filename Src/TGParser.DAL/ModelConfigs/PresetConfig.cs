using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGParser.DAL.Models;

namespace TGParser.DAL.ModelConfigs;

public class PresetConfig : IEntityTypeConfiguration<Preset>
{
    public void Configure(EntityTypeBuilder<Preset> builder)
    {
        builder.HasKey(k => k.PresetId);

        builder.Property(p => p.MinDateRegisterSeller).HasColumnType("date");
        builder.Property(p => p.MaxDateRegisterSeller).HasColumnType("date");
    }
}

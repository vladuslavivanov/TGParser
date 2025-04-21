using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGParser.DAL.Models;

namespace TGParser.DAL.ModelConfigs;

public class ProxyConfig : IEntityTypeConfiguration<Proxy>
{
    public void Configure(EntityTypeBuilder<Proxy> builder)
    {
        builder.HasKey(k => k.ProxyId);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGParser.DAL.Models;

namespace TGParser.DAL.ModelConfigs;

public class InvoiceConfig : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(k => k.InvoiceId);

        builder.HasOne(o => o.User)
            .WithMany(m => m.Invoices);
    }
}

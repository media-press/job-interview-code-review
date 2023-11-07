using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stapi.Worker.Models;

namespace Stapi.Worker.Services.Data.EntityTypeConfigurations;

internal class ImportMetadataEntityTypeConfiguration : IEntityTypeConfiguration<ImportMetadata>
{
    public void Configure(EntityTypeBuilder<ImportMetadata> builder)
    {
        builder.ToTable("metadata", "import");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Data)
            .HasColumnType("jsonb");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");
    }
}
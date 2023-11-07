using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stapi.Worker.Models;

namespace Stapi.Worker.Services.Data.EntityTypeConfigurations;

internal class LogEntityTypeConfiguration : IEntityTypeConfiguration<Log>
{
    public void Configure(EntityTypeBuilder<Log> builder)
    {
        builder.ToTable("logs", "import");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");
    }
}
using Microsoft.EntityFrameworkCore;
using Stapi.Worker.Models;
using Stapi.Worker.Services.Data.EntityTypeConfigurations;

namespace Stapi.Worker.Services.Data;

public class ImportDbContext : DbContext
{
    public DbSet<ImportMetadata> ImportMetadata { get; set; } = default!;

    public ImportDbContext(DbContextOptions<ImportDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ImportMetadataEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new LogEntityTypeConfiguration());
    }
}
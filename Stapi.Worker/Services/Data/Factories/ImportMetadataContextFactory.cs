using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stapi.Worker.Services.Data.Factories;

public class ImportMetadataContextFactory : IDesignTimeDbContextFactory<ImportDbContext>
{
    public ImportDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ImportDbContext>();
        optionsBuilder.UseNpgsql(args.Length >= 1 ? args[1] : "", x => x.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "import"))
            .UseSnakeCaseNamingConvention();

        return new ImportDbContext(optionsBuilder.Options);
    }
}
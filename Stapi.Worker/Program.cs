using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Stapi.Worker.Services.Data;
using Stapi.Worker.Services.Data.Interfaces;
using Stapi.Worker.Services.Repositories;
using Stapi.Worker.Workers;

namespace Stapi.Worker;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddDbContext<ImportDbContext>(options =>
                {
                    options.UseNpgsql("Host=postgres;Database=postgres;Username=postgres;Password=postgres", x => x.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "import"))
                        .UseSnakeCaseNamingConvention();
                });

                services.AddScoped<IUnitOfWork, UnitOfWork>()
                    .AddScoped<IImportRepository, ImportRepository>();

                services.AddHostedService<DataDownloadingWorker>();
                services.AddHostedService<DataProcessingWorker>();
            })
            .Build();

        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ImportDbContext>();
        await context.Database.MigrateAsync();

        await host.RunAsync();
    }
}
using Stapi.Worker.Models;
using Stapi.Worker.Services.Data;

namespace Stapi.Worker.Services.Repositories;

internal class ImportRepository : IImportRepository
{
    private readonly ImportDbContext _importDbContext;

    public ImportRepository(ImportDbContext importDbContext)
    {
        _importDbContext = importDbContext;
    }

    public async Task AddAsync(ImportMetadata importMetadata, CancellationToken cancellationToken)
    {
        await _importDbContext.ImportMetadata.AddAsync(importMetadata, cancellationToken)
            .ConfigureAwait(false);
    }
}
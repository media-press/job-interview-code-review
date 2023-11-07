using Stapi.Worker.Models;

namespace Stapi.Worker.Services.Repositories;

internal interface IImportRepository
{
    public Task AddAsync(ImportMetadata importMetadata, CancellationToken cancellationToken);
}
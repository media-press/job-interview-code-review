using Stapi.Worker.Services.Data.Interfaces;

namespace Stapi.Worker.Services.Data;

internal class UnitOfWork : IUnitOfWork
{
    private readonly ImportDbContext _importDbContext;

    public UnitOfWork(ImportDbContext importDbContext)
    {
        _importDbContext = importDbContext;
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await _importDbContext.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
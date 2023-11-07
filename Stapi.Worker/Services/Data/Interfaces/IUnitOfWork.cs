namespace Stapi.Worker.Services.Data.Interfaces;

internal interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
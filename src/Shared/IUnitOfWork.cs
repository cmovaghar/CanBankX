namespace Shared
{
    // Tiré du eShop (.NET 8)
    // https://github.com/dotnet/eShop/blob/release/8.0/src/Ordering.Domain/SeedWork/IUnitOfWork.cs
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
    }
}

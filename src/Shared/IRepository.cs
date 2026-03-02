namespace Shared
{
    // Tiré du eShop (.NET 8)
    // https://github.com/dotnet/eShop/blob/release/8.0/src/Ordering.Domain/SeedWork/IRepository.cs
    public interface IRepository<T> where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}

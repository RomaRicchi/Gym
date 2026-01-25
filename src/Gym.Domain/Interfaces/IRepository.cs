using Gym.Domain.Entities;

namespace Gym.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default);
    IQueryable<T> Query();
}

public interface IRepositoryPaged<T> : IRepository<T> where T : BaseEntity
{
    Task<(IReadOnlyList<T> Items, int Total)> GetPagedAsync(
        int page,
        int pageSize,
        string? search = null,
        CancellationToken ct = default);
}

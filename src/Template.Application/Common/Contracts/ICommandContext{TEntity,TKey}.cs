using Template.Domain.Common.Result;
using Template.Domain.Contracts;
using Template.Domain.ValueObjects;

namespace Template.Application.Common.Contracts
{
    public interface ICommandContext<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TKey : struct, IIdentity<TKey>
    {
        Task<IResult<TEntity>> AddAsync(TEntity entity, CancellationToken ct);
        Task UpdateAsync(TEntity entity, CancellationToken ct);
        Task RemoveAsync(TEntity entity, CancellationToken ct);
        Task<TEntity?> GetByIdAsync(TKey key, CancellationToken ct);
    }
}

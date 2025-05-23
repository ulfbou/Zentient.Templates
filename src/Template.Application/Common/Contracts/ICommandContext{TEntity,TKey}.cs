using MediatR;

using Template.Application.Features.Tenants.Commands;
using Template.Domain.Common.Result;
using Template.Domain.Contracts;
using Template.Domain.ValueObjects;

using Zentient.Results;

namespace Template.Application.Common.Contracts
{
    public interface ICommandContext<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TKey : struct, IIdentity<TKey>
    {
        Task<IResult<TEntity>> AddAsync(TEntity entity, CancellationToken ct);
        Task<IResult> UpdateAsync(TEntity entity, CancellationToken ct);
        Task<IResult> RemoveAsync(TEntity entity, CancellationToken ct);
        Task<IResult<TEntity>> GetByIdAsync(TKey key, CancellationToken ct);
        Task<IResult> ExistsByNameAsync(string name, CancellationToken none);
    }
}

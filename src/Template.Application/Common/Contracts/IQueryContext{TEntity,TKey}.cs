using Template.Domain.Contracts;
using Template.Domain.ValueObjects;

namespace Template.Application.Common.Contracts
{
    public interface IQueryContext<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TKey : struct, IIdentity<TKey>
    {
        IQueryable<TEntity> Query(bool asNoTracking = true, bool noCache = false, bool getSoftDelete = false);
    }
}

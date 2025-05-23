

using Template.Domain.Common.Result;
using Template.Domain.Entities;
using Template.Domain.ValueObjects;

using Zentient.Results;

namespace Template.Application.Common.Contracts
{
    public interface ITenantRepository
    {
        Task<IResult<Tenant>> AddAsync(Tenant tenant, bool commit, CancellationToken ct);
        Task<IResult<Tenant>> GetByIdAsync(TenantId tenantId, CancellationToken ct);
        Task<IResult> UpdateAsync(TenantId tenantId, string? name, IReadOnlyDictionary<string, string>? metadata, CancellationToken ct);
        Task<IResult> ExistsByNameAsync(string name, CancellationToken ct);
        Task<IResult> ExistsByIdAsync(TenantId tenantId, CancellationToken ct);
        Task<IResult<TenantId>> SoftDeleteAsync(TenantId tenantId, CancellationToken ct);
        Task<IResult> UpdateAsync(Tenant tenant, CancellationToken ct);
    }
}
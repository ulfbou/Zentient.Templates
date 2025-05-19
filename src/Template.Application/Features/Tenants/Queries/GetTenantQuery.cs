using MediatR;

using Template.Application.Common.Results;
using Template.Domain.Common.Result;
using Template.Domain.ValueObjects;

namespace Template.Application.Features.Tenants.Queries
{
    public record GetTenantQuery(
        TenantId TenantId,
        CancellationToken CancellationToken
    ) : IRequest<Result<TenantDto>>;

    public record TenantDto(
        TenantId Id,
        string Name,
        TenantStatus Status,
        IReadOnlyDictionary<string, string>? Metadata,
        DateTime CreatedOn
    );
}

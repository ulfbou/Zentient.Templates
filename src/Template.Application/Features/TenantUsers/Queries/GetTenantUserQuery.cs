using MediatR;

using Template.Application.Common;
using Template.Application.Common.Results;
using Template.Domain.Common.Result;
using Template.Domain.ValueObjects;

namespace Template.Application.Features.TenantUsers.Queries
{
    public record GetTenantUserQuery(
        TenantId TenantId,
        UserId UserId,
        CancellationToken CancellationToken
    ) : IRequest<IResult<TenantUserDto>>;

    public record GetTenantUsersQuery(
        TenantId TenantId,
        PagedQuery Paging,
        CancellationToken CancellationToken
    ) : IRequest<IResult<PaginatedList<TenantUserDto>>>;

    public record TenantUserDto(
        UserId UserId,
        string Email,
        string UserName,
        string Role
    );
}

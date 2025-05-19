using MediatR;

using Template.Application.Common.Results;
using Template.Domain.Common.Result;
using Template.Domain.ValueObjects;

namespace Template.Application.Features.TenantUsers.Commands
{
    public record CreateTenantUserCommand(
        TenantId TenantId,
        string Email,
        string Password,
        string Role,
        CancellationToken CancellationToken
    ) : IRequest<IResult<CreateTenantUserResponse>>;

    public record UpdateTenantUserCommand(
        TenantId TenantId,
        UserId UserId,
        string? Role,
        string? Password,
        CancellationToken CancellationToken
    ) : IRequest<IResult>;

    public record DeleteTenantUserCommand(
        UserId UserId,
        CancellationToken CancellationToken
    ) : IRequest<IResult>;

    public record RestoreTenantUserCommand(
        UserId UserId,
        CancellationToken CancellationToken
    ) : IRequest<IResult>;

    public record CreateTenantUserResponse(
        UserId UserId,
        string Role
    );
}

using MediatR;

using Template.Domain.ValueObjects;

using Zentient.Results;

namespace Template.Application.Features.TenantUsers.Commands
{
    public record CreateTenantUserCommand(
        TenantId TenantId,
        string Email,
        string Password,
        IReadOnlyList<string> Roles,
        CancellationToken CancellationToken
    ) : IRequest<IResult<CreateTenantUserResponse>>;

    public record UpdateTenantUserCommand(
        TenantId TenantId,
        UserId UserId,
        IReadOnlyList<string>? Roles,
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
        IReadOnlyList<string> Roles
    );
}

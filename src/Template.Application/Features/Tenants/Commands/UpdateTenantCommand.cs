using MediatR;

using Template.Application.Common.Results;
using Template.Domain.Common.Result;
using Template.Domain.ValueObjects;

using Zentient.Results;

namespace Template.Application.Features.Tenants.Commands
{
    public record CreateTenantCommand(
        string Name,
        string AdminEmail,
        string AdminPassword,
        Dictionary<string, string>? Metadata,
        CancellationToken CancellationToken
    ) : IRequest<IResult<CreateTenantResponse>>;

    public record UpdateTenantCommand(
        TenantId TenantId,
        string? Name,
        Dictionary<string, string>? Metadata,
        TenantStatus? Status,
        CancellationToken CancellationToken
    ) : IRequest<IResult<TenantId>>;

    public record DeleteTenantCommand(
        TenantId TenantId,
        CancellationToken CancellationToken
    ) : IRequest<IResult>;

    public record RestoreTenantCommand(
        TenantId TenantId,
        CancellationToken CancellationToken
    ) : IRequest<IResult>;

    public record CreateTenantResponse(
        TenantId TenantId,
        UserId AdminUserId,
        TenantStatus Status
    );
}

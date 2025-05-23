using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Template.Application.Common.Contracts;
using Template.Application.Common.Handlers;
using Template.Application.Common.Results;
using Template.Application.Features.Tenants.Commands;

using Template.Domain.Common;
using Template.Domain.Common.Result;
using Template.Domain.Contracts;
using Template.Domain.Entities;
using Template.Domain.ValueObjects;
using Template.Application.Common;

using Microsoft.EntityFrameworkCore;
using Zentient.Results;

namespace Template.Application.Features.Tenants
{
    public class CreateTenantCommandHandler : BaseCreateCommandHandler<CreateTenantCommand, Tenant, TenantId, CreateTenantResponse>, IRequestHandler<CreateTenantCommand, IResult<CreateTenantResponse>>
    {
        private readonly ICommandContext<TenantUser, UserId> _userCommandContext;
        private readonly IPasswordHasher _passwordHasher;

        public CreateTenantCommandHandler(
            ICommandContext<Tenant, TenantId> tenantCommandContext,
            ICommandContext<TenantUser, UserId> userCommandContext,
            IPasswordHasher passwordHasher,
            IUserContext userContext,
            IRequestContext requestContext,
            ActivitySource activitySource)
            : base(tenantCommandContext, userContext, requestContext, activitySource)
        {
            _userCommandContext = userCommandContext ?? throw new ArgumentNullException(nameof(userCommandContext));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        protected override Task<IResult<Tenant>> MapToEntityAsync(CreateTenantCommand command, CancellationToken ct)
        {
            var entity = Tenant.Create(command.Name, _userContext.UserId, command.Metadata);
            return Task.FromResult<IResult<Tenant>>(
                Result<Tenant>.Success(entity, string.Format(AppData.Tenants.SuccessTenantCreated, command.Name))
            );
        }

        protected override async Task<IResult<CreateTenantResponse>> MapToResponseAsync(Tenant entity, CancellationToken ct)
        {
            var response = new CreateTenantResponse(entity.Id, default, entity.Status);
            var message = string.Format(AppData.Tenants.SuccessTenantCreated, entity.Name);
            return await Task.FromResult(Result.Success<CreateTenantResponse>(response, message));
        }

        protected override async Task PerformPostCreationActionAsync(CreateTenantCommand command, Tenant createdEntity, CancellationToken ct)
        {
            var tenantId = createdEntity.Id;
            var name = command.Name;
            var email = command.AdminEmail;
            var hashedPassword = await _passwordHasher.HashPasswordAsync(command.AdminPassword);
            var adminUser = TenantUser.Create(createdEntity.Id, command.Name, command.AdminEmail, hashedPassword, [DomainData.Roles.SuperAdmin], _userContext.UserId);
            await _userCommandContext.AddAsync(adminUser, ct);
        }
    }

    public class UpdateTenantCommandHandler : BaseUpdateCommandHandler<UpdateTenantCommand, Tenant, TenantId>, IRequestHandler<UpdateTenantCommand, IResult>
    {
        public UpdateTenantCommandHandler(
            ICommandContext<Tenant, TenantId> context,
            IUserContext userContext,
            IRequestContext requestContext,
            ActivitySource activitySource)
            : base(context, userContext, requestContext, activitySource)
        { }

        protected override TenantId GetId(UpdateTenantCommand command) => command.TenantId;

        protected override async Task<IResult> PerformUpdateActionAsync(UpdateTenantCommand command, Tenant tenant, CancellationToken ct)
        {
            if (!string.IsNullOrEmpty(command.Name) && tenant.Name != command.Name)
            {
                var nameExists = await _context.ExistsByNameAsync(command.Name, ct);

                if (nameExists.IsSuccess)
                {
                    return AppData.Tenants.ErrorTenantNameAlreadyExists(command.Name);
                }

                tenant.UpdateName(command.Name, _userContext.UserId);
            }

            if (command.Metadata is not null)
            {
                tenant.UpdateMetadata(command.Metadata, _userContext.UserId);
            }

            if (command.Status is not null)
            {
                tenant.UpdateStatus(command.Status.Value, _userContext.UserId);
            }

            return Result.Success();
        }
    }

    public class DeleteTenantCommandHandler : BaseUpdateCommandHandler<DeleteTenantCommand, Tenant, TenantId>, IRequestHandler<DeleteTenantCommand, IResult>
    {
        public DeleteTenantCommandHandler(
            ICommandContext<Tenant, TenantId> context,
            IUserContext userContext,
            IRequestContext requestContext,
            ActivitySource activitySource)
            : base(context, userContext, requestContext, activitySource)
        { }

        protected override TenantId GetId(DeleteTenantCommand command) => command.TenantId;

        protected override Task<IResult> PerformUpdateActionAsync(DeleteTenantCommand command, Tenant existingEntity, CancellationToken ct)
        {
            existingEntity.MarkDeleted(_userContext.UserId);
            return Task.FromResult<IResult>(Result.Success());
        }
    }

    public class RestoreTenantCommandHandler : BaseUpdateCommandHandler<RestoreTenantCommand, Tenant, TenantId>, IRequestHandler<RestoreTenantCommand, IResult>
    {
        public RestoreTenantCommandHandler(
            ICommandContext<Tenant, TenantId> context,
            IUserContext userContext,
            IRequestContext requestContext,
            ActivitySource activitySource)
            : base(context, userContext, requestContext, activitySource)
        { }

        protected override TenantId GetId(RestoreTenantCommand command) => command.TenantId;

        protected override Task<IResult> PerformUpdateActionAsync(RestoreTenantCommand command, Tenant existingEntity, CancellationToken ct)
        {
            existingEntity.Restore(_userContext.UserId);
            return Task.FromResult<IResult>(Result.Success());
        }
    }
}

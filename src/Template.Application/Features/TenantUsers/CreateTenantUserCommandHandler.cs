using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using System.Diagnostics;

using Template.Application.Common;
using Template.Application.Common.Contracts;
using Template.Application.Common.Handlers;
using Template.Application.Common.Results;
using Template.Application.Features.TenantUsers.Commands;
using Template.Domain.Common.Result;
using Template.Domain.Contracts;
using Template.Domain.Entities;
using Template.Domain.ValueObjects;

using Zentient.Results;

namespace Template.Application.Features.TenantUsers
{
    public class CreateTenantUserCommandHandler : BaseCreateCommandHandler<CreateTenantUserCommand, TenantUser, UserId, CreateTenantUserResponse>
    {
        private readonly IPasswordHasher _passwordHasher;

        public CreateTenantUserCommandHandler(
            ICommandContext<TenantUser, UserId> context,
            IUserContext userContext,
            IRequestContext requestContext,
            ActivitySource activitySource,
            IPasswordHasher passwordHasher)
            : base(context, userContext, requestContext, activitySource)
        {
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        /// <inheritdoc />
        protected override async Task<IResult<TenantUser>> MapToEntityAsync(CreateTenantUserCommand command, CancellationToken ct)
        {
            // Domain rule: Check if email already exists for this tenant before creating
            var existsResult = await _commandContext.ExistsByNameAsync(command.Email, ct).ConfigureAwait(false);
            if (existsResult.IsSuccess)
            {
                return Result<TenantUser>.Failure(null, string.Format(AppData.Messages.TenantUserAlreadyExists, command.Email));
            }

            string hashedPassword = await _passwordHasher.HashPasswordAsync(command.Password);

            var entity = TenantUser.Create(
                command.TenantId,
                command.Email,
                command.Email,
                hashedPassword,
                command.Roles,
                _userContext.UserId
            );

            return Result<TenantUser>.Success(entity);
        }

        /// <inheritdoc />
        protected override Task<IResult<CreateTenantUserResponse>> MapToResponseAsync(TenantUser entity, CancellationToken ct)
        {
            var response = new CreateTenantUserResponse(entity.Id, entity.Roles);
            return Task.FromResult(Result.Success(response));
        }
    }

    public class UpdateTenantUserCommandHandler : BaseUpdateCommandHandler<UpdateTenantUserCommand, TenantUser, UserId>
    {
        private readonly IPasswordHasher _passwordHasher;

        public UpdateTenantUserCommandHandler(
            ICommandContext<TenantUser, UserId> context,
            IUserContext userContext,
            IRequestContext requestContext,
            ActivitySource activitySource,
            IPasswordHasher passwordHasher)
            : base(context, userContext, requestContext, activitySource)
        {
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        /// <inheritdoc />
        protected override UserId GetId(UpdateTenantUserCommand command) => command.UserId;

        /// <inheritdoc />
        protected override async Task<IResult> PerformUpdateActionAsync(UpdateTenantUserCommand command, TenantUser existingEntity, CancellationToken ct)
        {
            var roles = command.Roles ?? [];
            string? hashedPassword = null;

            if (command.Password != null)
            {
                hashedPassword = await _passwordHasher.HashPasswordAsync(command.Password);
            }

            existingEntity.Update(roles, hashedPassword, _userContext.UserId);

            return Result.Success();
        }
    }

    public class DeleteTenantUserCommandHandler : BaseUpdateCommandHandler<DeleteTenantUserCommand, TenantUser, UserId>
    {
        public DeleteTenantUserCommandHandler(
            ICommandContext<TenantUser, UserId> context,
            IUserContext userContext,
            IRequestContext requestContext,
            ActivitySource activitySource)
            : base(context, userContext, requestContext, activitySource)
        { }

        /// <inheritdoc />
        protected override UserId GetId(DeleteTenantUserCommand command) => command.UserId;

        /// <inheritdoc />
        protected override Task<IResult> PerformUpdateActionAsync(DeleteTenantUserCommand command, TenantUser existingEntity, CancellationToken ct)
        {
            existingEntity.MarkDeleted(_userContext.UserId);
            return Task.FromResult<IResult>(Result.Success());
        }
    }

    public class RestoreTenantUserCommandHandler : BaseUpdateCommandHandler<RestoreTenantUserCommand, TenantUser, UserId>
    {
        public RestoreTenantUserCommandHandler(
            ICommandContext<TenantUser, UserId> context,
            IUserContext userContext,
            IRequestContext requestContext,
            ActivitySource activitySource)
            : base(context, userContext, requestContext, activitySource)
        { }

        /// <inheritdoc />
        protected override UserId GetId(RestoreTenantUserCommand command) => command.UserId;

        /// <inheritdoc />
        protected override Task<IResult> PerformUpdateActionAsync(RestoreTenantUserCommand command, TenantUser existingEntity, CancellationToken ct)
        {
            existingEntity.Restore(_userContext.UserId);
            return Task.FromResult<IResult>(Result.Success());
        }
    }
}

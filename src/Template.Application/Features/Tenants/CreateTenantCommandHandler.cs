using MediatR;

using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;

using Template.Application.Common.Contracts;
using Template.Application.Common.Handlers;
using Template.Application.Common.Results;
using Template.Application.Features.Tenants.Commands;

using Template.Domain.Common;
using Template.Domain.Common.Result;
using Template.Domain.Entities;
using Template.Domain.ValueObjects;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Template.Application.Features.Tenants
{
    public class CreateTenantCommandHandler : BaseCreateCommandHandler<CreateTenantCommand, CreateTenantResponse, Tenant, TenantId>, IRequestHandler<CreateTenantCommand, IResult<CreateTenantResponse>>
    {
        private readonly ICommandContext<TenantUser, UserId> _userCommandContext;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserContext _userContext;

        public CreateTenantCommandHandler(
            ICommandContext<Tenant, TenantId> tenantCommandContext,
            ICommandContext<TenantUser, UserId> userCommandContext,
            IPasswordHasher passwordHasher,
            IUserContext userContext,
            ILogger<CreateTenantCommandHandler> logger)
            : base(tenantCommandContext, logger)
        {
            _userCommandContext = userCommandContext;
            _passwordHasher = passwordHasher;
            _userContext = userContext;
        }

        protected override TenantId GetId(CreateTenantCommand command) => Guid.Empty;

        protected override bool RequiresEntity(CreateTenantCommand command) => false;

        protected override async Task<IResult<CreateTenantResponse>> PerformAction(CreateTenantCommand command, CancellationToken ct)
        {
            var tenant = Tenant.Create(command.Name, _userContext.UserId, command.Metadata);
            var hashedPassword = _passwordHasher.HashPasswordAsync(command.AdminPassword);
            var adminUser = TenantUser.Create(tenant.Id, command.Name, command.AdminEmail, hashedPassword, DomainData.Roles.Admin, _userContext.UserId);

            var tenantResult = await _context.AddAsync(tenant, ct);
            var userResult = await _userCommandContext.AddAsync(adminUser, ct);

            if (tenantResult.IsFailure || userResult.IsFailure)
                return Result.Failure<CreateTenantResponse>(null, tenantResult.Errors.Concat(userResult.Errors));

            return Result.Success(new CreateTenantResponse(tenant.Id, adminUser.Id, tenant.Status));
        }

        protected override async Task<IResult> ValidateCommand(CreateTenantCommand command, CancellationToken ct)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(command.Name))
                errors.Add("Tenant name cannot be empty.");

            if (string.IsNullOrWhiteSpace(command.AdminEmail))
                errors.Add("Admin email cannot be empty.");

            if (string.IsNullOrWhiteSpace(command.AdminPassword))
                errors.Add("Admin password cannot be empty.");

            var nameExistsResult = await _context.ExistsByNameAsync(command.Name, ct);
            if (nameExistsResult.IsSuccess)
                errors.Add($"Tenant with the name '{command.Name}' already exists.");

            return errors.Count > 0 ? Result.Failure(string.Join(", ", errors)) : Result.Success();
        }
    }

    public class UpdateTenantCommandHandler : BaseUpdateCommandHandler<UpdateTenantCommand, Tenant, TenantId>, IRequestHandler<UpdateTenantCommand, IResult>
    {
        private readonly IUserContext _userContext;

        public UpdateTenantCommandHandler(
            ICommandContext<Tenant, TenantId> context,
            IUserContext userContext,
            ILogger<UpdateTenantCommandHandler> logger)
            : base(context, logger)
        {
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        protected override TenantId GetId(UpdateTenantCommand command) => throw new NotImplementedException();

        protected override async Task<IResult> PerformAction(UpdateTenantCommand command, CancellationToken ct)
        {
            IResult<Tenant> tenantResult = await _context.GetByIdAsync(command.TenantId, ct);

            if (tenantResult.IsFailure || tenantResult.Value == null)
                return tenantResult;

            var tenant = tenantResult.Value;
            if (!string.IsNullOrEmpty(command.Name) && tenant.Name != command.Name)
            {
                var nameExists = await _context.ExistsByNameAsync(command.Name, ct);
                if (nameExists.IsSuccess)
                {
                    return Result.Failure($"Tenant with the name '{command.Name}' already exists.");
                }

                tenant.UpdateName(command.Name);
            }

            if (command.Metadata is not null)
            {
                tenant.UpdateMetadata(command.Metadata, _userContext.UserId);
            }

            if (command.Status is not null)
            {
                tenant.UpdateStatus(command.Status.Value, _userContext.UserId);
            }

            return await _context.UpdateAsync(tenant, ct);
        }

        protected override bool RequiresEntity(UpdateTenantCommand command) => false;
    }

    public class DeleteTenantCommandHandler : IRequestHandler<DeleteTenantCommand, IResult>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        private readonly ILogger<DeleteTenantCommandHandler> _logger;

        public DeleteTenantCommandHandler(
            ITenantRepository tenantRepository,
            IUnitOfWork unitOfWork,
            IUserContext userContext,
            ILogger<DeleteTenantCommandHandler> logger)
        {
            _tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IResult> Handle(DeleteTenantCommand command, CancellationToken ct)
        {
            // 1. Retrieve the tenant by ID.
            var tenantResult = await _tenantRepository.GetByIdAsync(command.TenantId, ct);
            if (tenantResult.IsFailure)
            {
                _logger.LogError("Tenant with ID {TenantId} not found.", command.TenantId);
                return Result.Failure("Tenant not found.");
            }

            var tenant = tenantResult.Value;

            if (tenant == null)
            {
                _logger.LogError("Tenant with ID {TenantId} not found.", command.TenantId);
                return Result.Failure("Tenant not found.");
            }

            // 2. Soft delete the tenant (using the UserContext).
            tenant.MarkDeleted(_userContext.UserId);
            var result = await _tenantRepository.UpdateAsync(tenant, ct);

            // 3. Save the changes within a transaction.
            await _unitOfWork.BeginTransactionAsync(ct);

            try
            {
                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.CommitTransactionAsync(ct);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(ct);
                _logger.LogError(ex, "Error occurred while deleting tenant with ID {TenantId}.", command.TenantId);
                return Result.Failure($"Failed to delete tenant: {ex.Message}");
            }

            return Result.Success();
        }
    }

    public class RestoreTenantCommandHandler : IRequestHandler<RestoreTenantCommand, IResult>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        private readonly ILogger<RestoreTenantCommandHandler> _logger;
        public RestoreTenantCommandHandler(
            ITenantRepository tenantRepository,
            IUnitOfWork unitOfWork,
            IUserContext userContext,
            ILogger<RestoreTenantCommandHandler> logger)
        {
            _tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IResult> Handle(RestoreTenantCommand command, CancellationToken ct)
        {
            // 1. Retrieve the tenant by ID.
            var tenantResult = await _tenantRepository.GetByIdAsync(command.TenantId, ct);

            if (tenantResult.IsFailure)
            {
                _logger.LogError("Tenant with ID {TenantId} not found.", command.TenantId);
                return Result.Failure("Tenant not found.");
            }

            var tenant = tenantResult.Value;

            if (tenant == null)
            {
                _logger.LogError("Tenant with ID {TenantId} not found.", command.TenantId);
                return Result.Failure("Tenant not found.");
            }

            tenant.Restore(_userContext.UserId);
            var result = await _tenantRepository.UpdateAsync(tenant, ct);

            await _unitOfWork.BeginTransactionAsync(ct);

            try
            {
                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.CommitTransactionAsync(ct);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(ct);
                _logger.LogError(ex, "Error occurred while restoring tenant with ID {TenantId}.", command.TenantId);
                return Result.Failure($"Failed to restore tenant: {ex.Message}");
            }

            return Result.Success();
        }
    }
}

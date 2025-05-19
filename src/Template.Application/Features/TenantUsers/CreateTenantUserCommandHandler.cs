using MediatR;

using Microsoft.Extensions.Logging;

using Template.Application.Common.Contracts;
using Template.Application.Common.Handlers;
using Template.Application.Common.Results;
using Template.Application.Features.TenantUsers.Commands;
using Template.Domain.Common.Result;
using Template.Domain.Contracts;
using Template.Domain.Entities;
using Template.Domain.ValueObjects;

namespace Template.Application.Features.TenantUsers
{
    // -------------------------------
    // Tenant User Command Handlers
    // -------------------------------

    public class CreateTenantUserCommandHandler : BaseCreateCommandHandler<CreateTenantUserCommand, CreateTenantUserResponse, TenantUser, UserId>,
        IRequestHandler<CreateTenantUserCommand, Result<CreateTenantUserResponse>>
    {
        public CreateTenantUserCommandHandler(
            ICommandContext<TenantUser, UserId> context,
            ILogger<CreateTenantUserCommandHandler> logger)
            : base(context, logger)
        { }

        protected override async Task<IResult<CreateTenantUserResponse>> PerformAction(CreateTenantUserCommand command, CancellationToken ct)
        {
            // 1. Create User
            var user = new TenantUser(command.UserId, command.TenantId, command.Email, command.Role, command.Password);
            await _context.AddAsync(user, ct);
            return Result<CreateTenantUserResponse>.Success(new CreateTenantUserResponse(user.Id));
        }

    }
    public class UpdateTenantUserCommandHandler : IRequestHandler<UpdateTenantUserCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserContext _userContext;

        public UpdateTenantUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IUserContext userContext)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _userContext = userContext;
        }

        public async Task<Result> Handle(UpdateTenantUserCommand command, CancellationToken cancellationToken)
        {
            // 1. Get User
            var user = await _userRepository.GetByIdAsync(command.TenantId, command.UserId, cancellationToken);
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            // 2. Update properties
            string? hashedPassword = null;
            if (command.Password != null)
            {
                hashedPassword = _passwordHasher.HashPassword(command.Password);
            }
            user.Update(command.Role, hashedPassword, _userContext.UserId);

            // 3. Update in repository
            _userRepository.Update(user);

            // 4. Save
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }

    public class DeleteTenantUserCommandHandler : IRequestHandler<DeleteTenantUserCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;

        public DeleteTenantUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IUserContext userContext)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userContext = userContext;
        }

        public async Task<Result> Handle(DeleteTenantUserCommand command, CancellationToken cancellationToken)
        {
            // 1. Get User
            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            // 2. Delete (soft delete)
            user.Delete(_userContext.UserId);
            _userRepository.Update(user);

            // 3. Save
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }

    public class RestoreTenantUserCommandHandler : IRequestHandler<RestoreTenantUserCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;

        public RestoreTenantUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IUserContext userContext)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userContext = userContext;
        }

        public async Task<Result> Handle(RestoreTenantUserCommand command, CancellationToken cancellationToken)
        {
            // 1. Get User
            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            // 2. Restore  
            user.Restore(_userContext.UserId);
            _userRepository.Update(user);

            // 3. Save
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}


using System.ComponentModel.DataAnnotations;

using Template.Domain.Contracts.Validation;
using Template.Domain.Entities.Validation;
using Template.Domain.Events;
using Template.Domain.ValueObjects;

namespace Template.Domain.Entities
{
    /// <summary>
    /// Represents a user associated with a tenant.
    /// </summary>
    public sealed class TenantUser : TenantEntity<UserId>
    {
        /// <inheritdoc />
        public string UserName { get; private set; }

        /// <inheritdoc />
        public string Email { get; private set; }

        private readonly IValidator<TenantUser, UserId> _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantUser"/> class.
        /// </summary>
        /// <param name="id">The unique identifier of the tenant user.</param>
        /// <param name="tenantId">The unique identifier of the tenant.</param>
        /// <param name="userName">The username of the tenant user.</param>
        /// <param name="email">The email address of the tenant user.</param>
        /// <param name="createdBy">The user who created the tenant user.</param>
        /// <param name="validator">The validator for the tenant user.</param>
        private TenantUser(UserId id, TenantId tenantId, string userName, string email, string createdBy) : base(tenantId, id, createdBy)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            _validator = ValidatorFactory.Create<TenantUser, UserId>();
            _validator.Validate(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantUser"/> class.
        /// This constructor is used by Entity Framework Core for materialization.
        /// </summary>
#pragma warning disable CS8618
        private TenantUser() { }
#pragma warning restore CS8618

        /// <summary>
        /// Creates a new tenant user with the specified username and email.
        /// </summary>
        /// <param name="tenantId">The unique identifier of the tenant.</param>
        /// <param name="userName">The username of the tenant user.</param>
        /// <param name="email">The email address of the tenant user.</param>
        /// <param name="createdBy">The user who created the tenant user.</param>
        /// <param name="validator">The validator for the tenant user.</param>
        /// <returns>A new instance of the <see cref="TenantUser"/> class.</returns>
        public static TenantUser Create(TenantId tenantId, string userName, string email, string createdBy)
        {
            var newUser = new TenantUser(UserId.New(), tenantId, userName, email, createdBy);
            newUser.RaiseEvent(new TenantUserCreatedEvent(newUser));
            return newUser;
        }

        /// <summary>
        /// Sets the username of the tenant user.
        /// </summary>
        /// <param name="userName">The new username.</param>
        /// <param name="modifiedBy">The user who modified the tenant user.</param>
        public void SetUserName(string userName, string modifiedBy)
        {
            if (userName == UserName) return;
            string oldUserName = UserName;
            UserName = userName;
            _validator.Validate(this);
            MarkModified(modifiedBy);
            RaiseEvent(new TenantUserChangedNameEvent(this, UserName, oldUserName));
        }

        /// <inheritdoc />
        public void SetEmail(string email, string modifiedBy)
        {
            if (email == Email) return;
            string oldEmail = Email;
            Email = email;
            _validator.Validate(this);
            MarkModified(modifiedBy);
            RaiseEvent(new TenantUserChangedEmailEvent(this, Email, oldEmail));
        }

        public override void MarkDeleted()
        {
            if (!IsDeleted)
            {
                IsDeleted = true;
                DeletedOn = DateTime.UtcNow;
                RaiseEvent(new TenantUserDeletedEvent(this));
            }
        }

        public override void Restore()
        {
            if (IsDeleted)
            {
                IsDeleted = false;
                DeletedOn = null;
                RaiseEvent(new TenantUserRestoredEvent(this));
            }
        }
    }
}

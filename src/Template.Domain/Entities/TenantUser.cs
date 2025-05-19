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
        private IValidator<TenantUser, UserId> _validator;

        /// <inheritdoc />
        public string UserName
        {
            get => _userName;
            private set
            {
                _userName = value;
            }
        }
        private string _userName = string.Empty;

        /// <inheritdoc />
        public string Email
        {
            get => _email;
            private set
            {
                _email = value;
            }
        }
        private string _email = string.Empty;

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
            UserName = !string.IsNullOrWhiteSpace(userName) ? userName : throw new ArgumentNullException(nameof(userName));
            Email = !string.IsNullOrWhiteSpace(email) ? email : throw new ArgumentNullException(nameof(email));
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
        public static TenantUser Create(TenantId tenantId, string userName, string email, string passwordHash, string initialRole, string createdBy)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            TenantUser newUser = new TenantUser(UserId.New(), tenantId, userName, email, createdBy);
            newUser.RaiseEvent(new TenantUserCreatedEvent(newUser.Id));
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
            RaiseEvent(new TenantUserChangedNameEvent(Id, UserName, oldUserName));
        }

        /// <inheritdoc />
        public void SetEmail(string email, string modifiedBy)
        {
            if (email == Email) return;
            string oldEmail = Email;
            Email = email;
            _validator.Validate(this);
            MarkModified(modifiedBy);
            RaiseEvent(new TenantUserChangedEmailEvent(Id, Email, oldEmail));
        }

        public override void MarkDeleted(string userId)
        {
            if (!IsDeleted)
            {
                IsDeleted = true;
                ModifiedBy = userId;
                ModifiedOn = DateTime.UtcNow;
                DeletedOn = ModifiedOn;
                RaiseEvent(new TenantUserDeletedEvent(Id));
            }
        }

        public override void Restore(string userId)
        {
            if (IsDeleted)
            {
                IsDeleted = false;
                ModifiedBy = userId;
                ModifiedOn = DateTime.UtcNow;
                DeletedOn = null;
                RaiseEvent(new TenantUserRestoredEvent(Id));
            }
        }
    }
}

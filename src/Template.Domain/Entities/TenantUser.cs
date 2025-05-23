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

        public string PasswordHash
        {
            get => _passwordHash;
            private set
            {
                _passwordHash = value;
            }
        }
        private string _passwordHash = string.Empty;

        /// <inheritdoc />
        public IReadOnlyList<string> Roles
        {
            get => _roles;
            private set
            {
                _roles = value;
            }
        }
        private IReadOnlyList<string> _roles = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantUser"/> class.
        /// </summary>
        /// <param name="id">The unique identifier of the tenant user.</param>
        /// <param name="tenantId">The unique identifier of the tenant.</param>
        /// <param name="userName">The username of the tenant user.</param>
        /// <param name="email">The email address of the tenant user.</param>
        /// <param name="createdBy">The user who created the tenant user.</param>
        /// <param name="validator">The validator for the tenant user.</param>
        private TenantUser(UserId id, TenantId tenantId, string userName, string email, IReadOnlyList<string>? initialRoles, string createdBy) : base(tenantId, id, createdBy)
        {
            UserName = !string.IsNullOrWhiteSpace(userName) ? userName : throw new ArgumentNullException(nameof(userName));
            Email = !string.IsNullOrWhiteSpace(email) ? email : throw new ArgumentNullException(nameof(email));
            Roles = initialRoles ?? new List<string>();
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
        public static TenantUser Create(TenantId tenantId, string userName, string email, string passwordHash, IEnumerable<string>? initialRoles, string createdBy)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                throw new ArgumentNullException(nameof(passwordHash));
            }

            if (initialRoles != null)
            {
                var roles = new List<string>();
                var errors = new List<string>();
                foreach (var role in initialRoles)
                {
                    if (string.IsNullOrWhiteSpace(role))
                    {
                        errors.Add("Role cannot be null or whitespace.");
                        continue;
                    }

                    roles.Add(role);
                }

                if (errors.Count > 0)
                {
                    throw new ArgumentException(string.Join("; ", errors), nameof(initialRoles));
                }
            }


            TenantUser newUser = new TenantUser(UserId.New(), tenantId, userName, email, initialRoles?.ToList(), createdBy);
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

        /// <inheritdoc />
        public void SetRoles(IReadOnlyList<string> roles, string modifiedBy)
        {
            if (roles is null || roles.Count == 0) return;

            var orderedRoles = roles.Order();

            if (_roles != null)
            {
                if (_roles.All(r => roles.Contains(r)) && roles.All(r => _roles.Contains(r)))
                {
                    return;
                }
            }

            var oldRoles = _roles;
            Roles = orderedRoles.ToList();
            _validator.Validate(this);
            MarkModified(modifiedBy);
            RaiseEvent(new TenantUserChangedRolesEvent(Id, Roles, oldRoles));
        }

        /// <inheritdoc />
        public void Update(IReadOnlyList<string> roles, string? passwordHash, string modifiedBy)
        {
            if (roles != null)
            {
                SetRoles(roles, modifiedBy);
            }
            if (passwordHash != null)
            {
                SetPassword(passwordHash, modifiedBy);
            }
        }

        /// <inheritdoc />
        public void SetPassword(string passwordHash, string modifiedBy)
        {
            if (string.IsNullOrWhiteSpace(passwordHash)) return;
            if (passwordHash == PasswordHash) return;
            PasswordHash = passwordHash;
            MarkModified(modifiedBy);
            // RaiseEvent(new TenantUserChangedPasswordEvent(Id, PasswordHash));
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

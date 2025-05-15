using System.Text.RegularExpressions;

using Template.Domain.Events;
using Template.Domain.ValueObjects;

namespace Template.Domain.Entities
{
    /// <summary>
    /// Represents a user associated with a tenant.
    /// </summary>
    public sealed class TenantUser : TenantEntity<UserId>
    {
        public string UserName { get; private set; }
        public string Email { get; private set; }

        private TenantUser(UserId id, TenantId tenantId, string userName, string email, string createdBy) : base(tenantId, id, createdBy)
        {
            UserName = userName;
            Email = email;
        }

        private TenantUser() { }

        public static TenantUser Create(TenantId tenantId, string userName, string email, string createdBy)
        {
            // Validation Logic
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentNullException(nameof(userName));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException(nameof(email));
            if (!Regex.IsMatch(userName, @"^[a-zA-Z0-9._-]{3,}$"))
                throw new ArgumentException("Invalid username format.", nameof(userName));
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("Invalid email format.", nameof(email));

            var newUser = new TenantUser(UserId.New(), tenantId, userName, email, createdBy);
            newUser.RaiseEvent(new TenantUserCreatedEvent(newUser.Id, tenantId, userName));
            return newUser;
        }

        public void SetUserName(string userName, string modifiedBy)
        {
            if (string.IsNullOrWhiteSpace(userName) || userName == UserName) return;
            if (!Regex.IsMatch(userName, @"^[a-zA-Z0-9._-]{3,}$"))
            {
                throw new ArgumentException("Invalid username format. Must be at least 3 characters long and can contain letters, numbers, dots, underscores, and hyphens.");
            }
            string oldUserName = UserName;
            UserName = userName;
            MarkModified(modifiedBy);
            RaiseEvent(new TenantUserChangedNameEvent(Id, TenantId, UserName, oldUserName));
        }

        public void SetEmail(string email, string modifiedBy)
        {
            if (string.IsNullOrWhiteSpace(email) || email == Email) return;
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new ArgumentException("Invalid email format.");
            }

            string oldEmail = Email;
            Email = email;
            MarkModified(modifiedBy);
            RaiseEvent(new TenantUserChangedEmailEvent(Id, TenantId, Email, oldEmail));
        }

        public override void MarkDeleted()
        {
            if (!IsDeleted)
            {
                IsDeleted = true;
                DeletedOn = DateTime.UtcNow;
                RaiseEvent(new TenantUserDeletedEvent(Id, TenantId, UserName));
            }
        }

        public override void Restore()
        {
            if (IsDeleted)
            {
                IsDeleted = false;
                DeletedOn = null;
                RaiseEvent(new TenantUserRestoredEvent(Id, TenantId, UserName));
            }
        }
    }
}

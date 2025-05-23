using System.Collections.Immutable;

using Template.Domain.Common.Result;
using Template.Domain.Contracts.Validation;
using Template.Domain.Contracts;
using Template.Domain.Entities.Validation;
using Template.Domain.Events;
using Template.Domain.ValueObjects;

namespace Template.Domain.Entities
{
    /// <summary>
    /// Represents a tenant in the application.
    /// </summary>
    public sealed class Tenant : AggregateRoot<TenantId>, ITenant
    {
        private readonly IValidator<Tenant, TenantId> _validator;

        /// <inheritdoc />
        public string Name
        {
            get => _name;
            private set
            {
                _name = value;
            }
        }
        private string _name = string.Empty;

        /// <inheritdoc />
        public TenantStatus Status
        {
            get => _status;
            private set
            {
                _status = value;
            }
        }
        private TenantStatus _status = TenantStatus.Inactive;

        /// <inheritdoc />
        public IReadOnlyDictionary<string, string> Metadata { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tenant"/> class.
        /// </summary>
        /// <param name="id">The unique identifier of the tenant.</param>
        /// <param name="name">The name of the tenant.</param>
        /// <param name="createdBy">The user who created the tenant.</param>
        /// <param name="metadata">Additional metadata associated with the tenant.</param>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="name"/> or <paramref name="createdBy"/> is <see langword="null"/> or empty.</exception>
        private Tenant(TenantId id, string name, string createdBy, Dictionary<string, string> metadata) : base(id, createdBy)
        {
            Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentNullException(nameof(name));
            Metadata = metadata.ToImmutableDictionary();
            _validator = ValidatorFactory.Create<Tenant, TenantId>();
            _validator.Validate(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tenant"/> class.
        /// This constructor is used by Entity Framework Core for materialization.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public Tenant() : base() { /* For EF Core */ }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        /// <summary>
        /// Creates a new tenant with the specified name and metadata.
        /// </summary>
        /// <param name="name">The name of the tenant.</param>
        /// <param name="createdBy">The user who created the tenant.</param>
        /// <param name="metadata">Additional metadata associated with the tenant.</param>
        /// <returns>A new instance of the <see cref="Tenant"/> class.</returns>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="name"/> or <paramref name="createdBy"/> is <see langword="null"/> or empty.</exception>
        /// <remarks>
        /// Upon creation, the tenant is in an active state, and the <see cref="TenantCreatedEvent"/> is raised.
        /// </remarks>
        public static Tenant Create(string name, string createdBy, Dictionary<string, string>? metadata = null)
        {
            var tenant = new Tenant(TenantId.New(), name, createdBy, metadata ?? new Dictionary<string, string>());
            tenant.RaiseEvent(new TenantCreatedEvent(tenant.Id));
            return tenant;
        }

        /// <inheritdoc />
        public void UpdateName(string newName, string updatedBy)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentNullException(nameof(newName), "Tenant name cannot be null or empty.");
            }

            if (newName == Name) return;

            string previousName = Name;
            Name = newName;
            ModifiedBy = updatedBy;
            ModifiedOn = DateTime.UtcNow;
            _validator.Validate(this);
            RaiseEvent(new TenantUpdatedEvent(Id));
        }

        /// <inheritdoc />
        public void UpdateMetadata(Dictionary<string, string>? newMetadata, UserId modifiedBy)
        {
            if (newMetadata == null)
            {
                newMetadata = new Dictionary<string, string>();
            }

            // Determine if there is a change.
            bool hasChanged = newMetadata.Count != Metadata.Count ||
                              !newMetadata.All(kvp => Metadata.ContainsKey(kvp.Key) && Metadata[kvp.Key] == kvp.Value);

            if (hasChanged)
            {
                Metadata = newMetadata.ToImmutableDictionary();
                ModifiedBy = modifiedBy;
                ModifiedOn = DateTime.UtcNow;
                RaiseEvent(new TenantMetadataUpdatedEvent(Id, Metadata));
            }
        }

        /// <inheritdoc />
        public override void MarkDeleted(string userId)
        {
            if (!IsDeleted)
            {
                IsDeleted = true;
                ModifiedBy = userId;
                DeletedOn = DateTime.UtcNow;
                ModifiedOn = DeletedOn;
                Status = TenantStatus.Deleted;
                RaiseEvent(new TenantDeletedEvent(Id));
            }
        }

        /// <inheritdoc />
        public override void Restore(string userId)
        {
            if (IsDeleted)
            {
                IsDeleted = false;
                ModifiedBy = userId;
                ModifiedOn = DateTime.UtcNow;
                DeletedOn = null;
                Status = TenantStatus.Active;
                RaiseEvent(new TenantRestoredEvent(Id));
            }
        }

        public void UpdateStatus(TenantStatus value, UserId userId)
        {
            if (!IsDeleted)
            {
                if (value != Status && value != TenantStatus.Deleted)
                {
                    Status = value;
                    ModifiedBy = userId;
                    ModifiedOn = DateTime.UtcNow;
                    RaiseEvent(new TenantStatusUpdatedEvent(Id, Status));
                }
            }
        }
    }
}

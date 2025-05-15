using System.Collections.Immutable;

using Template.Domain.Common.Exceptions;
using Template.Domain.Contracts;
using Template.Domain.Contracts.Validation;
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
            tenant.RaiseEvent(new TenantCreatedEvent(tenant));
            return tenant;
        }

        /// <inheritdoc />
        public void UpdateName(string newName)
        {
            if (newName == Name) return;
            string previousName = Name;
            Name = newName;
            _validator.Validate(this);
            RaiseEvent(new TenantUpdatedEvent(this));
        }

        /// <inheritdoc />
        public override void MarkDeleted()
        {
            if (!IsDeleted)
            {
                IsDeleted = true;
                DeletedOn = DateTime.UtcNow;
                RaiseEvent(new TenantDeletedEvent(this));
            }
        }

        /// <inheritdoc />
        public override void Restore()
        {
            if (IsDeleted)
            {
                IsDeleted = false;
                DeletedOn = null;
                RaiseEvent(new TenantRestoredEvent(this));
            }
        }
    }
}

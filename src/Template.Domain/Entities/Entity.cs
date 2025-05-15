using Template.Domain.Contracts;
using Template.Domain.Events;
using Template.Domain.ValueObjects;

namespace Template.Domain.Entities
{
    /// <summary>
    /// Base class for all entities with a strongly-typed key.
    /// </summary>
    /// typeparam name="TKey">The type of the key.</typeparam>
    public abstract class Entity<TKey> : IEntity<TKey>, IAuditable, ISoftDelete, IHasDomainEvents
            where TKey : struct, IIdentity<TKey>
    {
        private readonly HashSet<IDomainEvent> _domainEvents = new();

        /// <inheritdoc />
        public TKey Id { get; protected set; }

        /// <inheritdoc />
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        /// <inheritdoc />
        public DateTime CreatedOn { get; protected init; }

        /// <inheritdoc />
        public string CreatedBy { get; protected init; } = string.Empty;

        /// <inheritdoc />
        public DateTime? ModifiedOn { get; protected set; }

        /// <inheritdoc />
        public string? ModifiedBy { get; protected set; }

        /// <inheritdoc />
        public bool IsDeleted { get; protected set; }

        /// <inheritdoc />
        public DateTime? DeletedOn { get; protected set; }

        /// <inheritdoc />
        public IReadOnlySet<IDomainEvent> DomainEvents => _domainEvents;

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity{TKey}"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="createdBy">The user who created the entity.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="createdBy"/> is <see langword="null"/> or empty.</exception>
        protected Entity(TKey id, string createdBy)
        {
            if (string.IsNullOrWhiteSpace(createdBy))
            {
                throw new ArgumentException("CreatedBy cannot be null or empty.", nameof(createdBy));
            }

            Id = id;
            CreatedOn = DateTime.UtcNow;
            CreatedBy = createdBy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity{TKey}"/> class.
        /// This constructor is used by Entity Framework Core for materialization.
        /// </summary>
        protected Entity() { /* For EF Core */ }

        /// <inheritdoc />
        public void ClearDomainEvents() => _domainEvents.Clear();

        /// <inheritdoc />
        public void RaiseEvent(IDomainEvent eventItem)
        {
            if (eventItem == null) throw new ArgumentNullException(nameof(eventItem));
            _domainEvents.Add(eventItem);
        }

        /// <inheritdoc />
        public virtual void MarkDeleted()
        {
            if (!IsDeleted)
            {
                IsDeleted = true;
                DeletedOn = DateTime.UtcNow;
                RaiseEvent(new EntitySoftDeletedEvent<TKey>(this, DeletedOn.Value));
            }
        }

        /// <inheritdoc />
        public virtual void Restore()
        {
            if (IsDeleted)
            {
                IsDeleted = false;
                var restoredOn = DateTime.UtcNow;
                DeletedOn = null;
                RaiseEvent(new EntityRestoredEvent<TKey>(this, restoredOn));
            }
        }
    }
}

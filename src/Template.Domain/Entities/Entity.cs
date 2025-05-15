using Template.Domain.Contracts;
using Template.Domain.Events;
using Template.Domain.ValueObjects;

namespace Template.Domain.Entities
{
    public abstract class Entity<TKey> : IEntity<TKey>, IAuditable, ISoftDelete, IHasDomainEvents
            where TKey : struct, IIdentity<TKey>
    {
        public TKey Id { get; protected set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public DateTime CreatedOn { get; protected init; }
        public string CreatedBy { get; protected init; } = string.Empty;
        public DateTime? ModifiedOn { get; protected set; }
        public string? ModifiedBy { get; protected set; }
        public bool IsDeleted { get; protected set; }
        public DateTime? DeletedOn { get; protected set; }
        private readonly HashSet<IDomainEvent> _domainEvents = new();
        public IReadOnlySet<IDomainEvent> DomainEvents => _domainEvents;

        protected Entity(TKey id, string createdBy)
        {
            Id = id;
            CreatedOn = DateTime.UtcNow;
            CreatedBy = createdBy;
        }

        protected Entity() { /* For EF Core */ }

        public void ClearDomainEvents() => _domainEvents.Clear();
        public void RaiseEvent(IDomainEvent eventItem)
        {
            if (eventItem == null) throw new ArgumentNullException(nameof(eventItem));
            _domainEvents.Add(eventItem);
        }

        public virtual void MarkDeleted()
        {
            if (!IsDeleted)
            {
                IsDeleted = true;
                DeletedOn = DateTime.UtcNow;
                RaiseEvent(new EntitySoftDeletedEvent<TKey>(Id, DeletedOn.Value));
            }
        }

        public virtual void Restore()
        {
            if (IsDeleted)
            {
                IsDeleted = false;
                var restoredOn = DateTime.UtcNow;
                DeletedOn = null;
                RaiseEvent(new EntityRestoredEvent<TKey>(Id, restoredOn));
            }
        }
    }
}

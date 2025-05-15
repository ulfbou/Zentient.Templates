using System.Collections.Immutable;

using Template.Domain.Contracts;
using Template.Domain.Events;
using Template.Domain.ValueObjects;

namespace Template.Domain.Entities
{

    public sealed class Tenant : AggregateRoot<TenantId>, ITenant
    {
        public string Name { get; private set; }
        public IReadOnlyDictionary<string, string> Metadata { get; private set; }

        private Tenant(TenantId id, string name, string createdBy, Dictionary<string, string> metadata) : base(id, createdBy)
        {
            Name = name;
            Metadata = metadata.ToImmutableDictionary();
        }

        public static Tenant Create(string name, string createdBy, Dictionary<string, string>? metadata = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Tenant name cannot be null or empty.", nameof(name));
            }

            var tenant = new Tenant(TenantId.New(), name, createdBy, metadata ?? new Dictionary<string, string>());
            tenant.RaiseEvent(new TenantCreatedEvent(tenant.Id, tenant.Name));
            return tenant;
        }

        private Tenant() { }

        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName) || newName == Name)
            {
                return;
            }
            string previousName = Name;
            Name = newName;
            RaiseEvent(new TenantUpdatedEvent(Id, Name));
        }

        public override void MarkDeleted()
        {
            if (!IsDeleted)
            {
                IsDeleted = true;
                DeletedOn = DateTime.UtcNow;
                RaiseEvent(new TenantDeletedEvent(Id, Name));
            }
        }
        public override void Restore()
        {
            if (IsDeleted)
            {
                IsDeleted = false;
                DeletedOn = null;
                RaiseEvent(new TenantRestoredEvent(Id, Name));
            }
        }
    }
}

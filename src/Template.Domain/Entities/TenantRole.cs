using Template.Domain.Contracts.Validation;
using Template.Domain.Entities.Validation;
using Template.Domain.ValueObjects;

namespace Template.Domain.Entities
{
    public sealed class TenantRole : TenantEntity<RoleId>
    {
        private IValidator<TenantRole, RoleId> _validator;
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public TenantRole(TenantId tenantId, RoleId id, string name, string description, string createdBy) : base(tenantId, id, createdBy)
        {
            Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentNullException(nameof(name));
            Description = description;
            _validator = ValidatorFactory.Create<TenantRole, RoleId>();
            _validator.Validate(this);
        }
    }
}

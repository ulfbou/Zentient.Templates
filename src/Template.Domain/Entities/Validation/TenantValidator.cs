using Template.Domain.Common.Exceptions;
using Template.Domain.Contracts.Validation;
using Template.Domain.ValueObjects;

namespace Template.Domain.Entities.Validation
{
    // Dedicated Validator Classes
    public class TenantValidator : IValidator<Tenant, TenantId>
    {
        public void Validate(Tenant entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                throw new DomainValidationException("Tenant name cannot be null or empty.");
            }
        }
    }
}

using Template.Domain.Common.Exceptions;
using Template.Domain.Contracts.Validation;
using Template.Domain.ValueObjects;

namespace Template.Domain.Entities.Validation
{
    /// <summary>
    /// Validator for the <see cref="Tenant"/> entity.
    /// </summary>
    public class TenantValidator : IValidator<Tenant, TenantId>
    {
        /// <inheritdoc />
        public void Validate(Tenant entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                throw new DomainValidationException("Tenant name cannot be null or empty.");
            }
        }
    }
}

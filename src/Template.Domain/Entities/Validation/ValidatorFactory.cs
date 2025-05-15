using Template.Domain.Contracts;
using Template.Domain.Contracts.Validation;
using Template.Domain.ValueObjects;

namespace Template.Domain.Entities.Validation
{
    /// <summary>
    /// Factory class for creating validators for tenant entities.
    /// </summary>
    public static class ValidatorFactory
    {
        private static readonly Dictionary<Type, IValidator> _validators = new Dictionary<Type, IValidator>
        {
            { typeof(Tenant), new TenantValidator() },
            { typeof(TenantUser), new TenantUserValidator() },
        };

        /// <summary>
        /// Gets the validator for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <returns>A <see cref="IValidator{TEntity, TKey}"/> instance for the specified entity type.</returns>
        /// <exception cref="NotSupportedException">Thrown when the entity type is not supported.</exception>
        public static IValidator<TEntity, TKey> Create<TEntity, TKey>()
            where TEntity : class, ITenantEntity<TKey>
            where TKey : struct, IIdentity<TKey>
        {
            if (_validators.TryGetValue(typeof(TEntity), out IValidator? validator))
            {
                return (IValidator<TEntity, TKey>)validator;
            }

            throw new NotSupportedException($"Validator for {typeof(TEntity).Name} is not supported.");
        }
    }
}

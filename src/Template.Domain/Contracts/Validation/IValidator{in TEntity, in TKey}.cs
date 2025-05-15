using Template.Domain.Common.Exceptions;
using Template.Domain.ValueObjects;

namespace Template.Domain.Contracts.Validation
{
    /// <summary>
    /// Defines a contract for validating an object.
    /// </summary>
    /// <typeparam name="T">The type of object to validate.</typeparam>
    public interface IValidator<in TEntity, in TKey> : IValidator
        where TEntity : class, IEntity<TKey>
        where TKey : struct, IIdentity<TKey>
    {
        /// <summary>
        /// Validates the specified entity. 
        /// </summary>
        /// <param name="entity">The object to validate.</param>
        /// <exception cref="DomainValidationException">Thrown when the object is invalid.</exception>
        void Validate(TEntity entity);
    }
}

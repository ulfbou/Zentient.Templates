using Template.Domain.Entities;
using Template.Domain.ValueObjects;

namespace Template.Domain.Contracts
{
    /// <summary>
    /// Represents a tenant in the system with a unique identifier, name, and metadata.
    /// </summary>
    public interface ITenant : IEntity<TenantId>
    {
        /// <summary>
        /// Gets the unique identifier of the tenant.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the metadata associated with the tenant.
        /// </summary>
        IReadOnlyDictionary<string, string> Metadata { get; }

        /// <summary>
        /// Updates the name of the tenant.
        /// </summary>
        /// <param name="newName">The new name for the tenant.</param>
        void UpdateName(string newName);
    }
}

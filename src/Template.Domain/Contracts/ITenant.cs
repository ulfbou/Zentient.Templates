using Template.Domain.Common.Result;
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
        TenantStatus Status { get; }

        /// <summary>
        /// Updates the name of the tenant.
        /// </summary>
        /// <param name="newName">The new name of the tenant.</param>
        /// <remarks>
        /// If the new name is the same as the current name or is <see langword="null"/> or empty, no action is taken.
        /// The <see cref="TenantUpdatedEvent"/> is raised if the name is changed.
        /// </remarks>
        void UpdateName(string newName);

        /// <summary>
        /// Updates the metadata of the tenant.
        /// </summary>
        /// <param name="newMetadata">The new metadata to be associated with the tenant.</param>
        /// <param name="modifiedBy">The user who modified the metadata.</param>
        void UpdateMetadata(Dictionary<string, string>? newMetadata, UserId modifiedBy);
    }
}

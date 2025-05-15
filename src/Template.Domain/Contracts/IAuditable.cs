namespace Template.Domain.Contracts
{
    /// <summary>
    /// Base interface for all entities.
    /// </summary>
    public interface IAuditable
    {
        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        DateTime CreatedOn { get; }

        /// <summary>
        /// Gets or sets the user who created the entity.
        /// </summary>
        string CreatedBy { get; }

        /// <summary>
        /// Gets or sets the date and time when the entity was last modified.
        /// </summary>
        DateTime? ModifiedOn { get; }

        /// <summary>
        /// Gets or sets the user who last modified the entity.
        /// </summary>
        string? ModifiedBy { get; }
    }
}

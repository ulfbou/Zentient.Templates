namespace Template.Domain.Contracts
{
    /// <summary>
    /// Interface for entities that support soft deletion.
    /// </summary>
    public interface ISoftDelete
    {
        /// <summary>
        /// Gets a value indicating whether the entity is deleted.
        /// </summary>
        bool IsDeleted { get; }

        /// <summary>
        /// Gets the date and time when the entity was deleted.
        /// </summary>
        DateTime? DeletedOn { get; }

        /// <summary>
        /// Marks the entity as deleted.
        /// </summary>
        void MarkDeleted();

        /// <summary>
        /// Restores the entity from a soft delete.
        /// </summary>
        /// <remarks>
        /// This method should be called to restore the entity after it has been marked as deleted.
        /// An <see cref="IDomainEvent"/> is raised to indicate that the entity has been restored.
        /// </remarks>
        void Restore();
    }
}

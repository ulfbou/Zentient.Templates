namespace Template.Domain.Contracts
{
    /// <summary>
    /// Base interface for all entities.
    /// </summary>
    public interface IEntity : IAuditable, ISoftDelete
    {
        /// <summary>
        /// Gets or sets the row version for concurrency checks.
        /// </summary>
        byte[] RowVersion { get; set; }
    }
}

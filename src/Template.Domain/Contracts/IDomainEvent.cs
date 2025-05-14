namespace Template.Domain.Contracts
{
    /// <summary>
    /// Represents a domain event that can be raised by an entity.
    /// </summary>
    public interface IDomainEvent
    {
        /// <summary>
        /// Gets the unique identifier of the event.
        /// </summary>
        DateTime OccurredOn { get; }
    }
}

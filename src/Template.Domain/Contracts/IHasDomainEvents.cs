namespace Template.Domain.Contracts
{
    /// <summary>
    /// Interface for domain entities that can raise domain events.
    /// </summary>
    public interface IHasDomainEvents
    {
        /// <summary>
        /// Gets the collection of domain events raised by the entity.
        /// </summary>
        IReadOnlySet<IDomainEvent> DomainEvents { get; }

        /// <summary>
        /// Clears the domain events after they have been handled.
        /// </summary>
        void ClearDomainEvents();

        /// <summary>
        /// Raises a domain event.
        /// </summary>
        /// <param name="domainEvent">The domain event to raise.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="domainEvent"/> is null.</exception>
        public void RaiseEvent(IDomainEvent domainEvent);
    }
}

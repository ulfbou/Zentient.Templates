namespace Template.Domain.Contracts
{
    /// <summary>
    /// Interface for handling domain events.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event.</typeparam>
    public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        /// <summary>
        /// Handles the domain event.
        /// </summary>
        /// <param name="event">The domain event to handle.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task Handle(TEvent @event, CancellationToken cancellationToken);
    }
}

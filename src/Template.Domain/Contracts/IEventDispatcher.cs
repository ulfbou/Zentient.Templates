namespace Template.Domain.Contracts
{
    /// <summary>
    /// Interface for dispatching domain events.
    /// </summary>
    public interface IEventDispatcher
    {
        /// <summary>
        /// Dispatches a collection of domain events.
        /// </summary>
        /// <param name="events">The collection of domain events to dispatch.</param>
        /// <param name="cancellationToken">The cancellation token. Optional. Default is <see cref="CancellationToken.None"/>.</param>
        Task Dispatch(IReadOnlyCollection<IDomainEvent> events, CancellationToken cancellationToken = default);
    }
}

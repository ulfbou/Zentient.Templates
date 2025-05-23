using MediatR;

using Microsoft.Extensions.Logging;

using Template.Application.Common.Contracts;

namespace Template.Application.Common.Behaviors
{
    /// <summary>
    /// Pipeline behavior that wraps the request execution in a database transaction using <see cref="IUnitOfWork"/>.
    /// Commits the transaction if the request succeeds, otherwise rolls back on exception.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public sealed class TransactionBehavior<TRequest, TResponse>
        : PipelineBehaviorBase<TRequest, TResponse>
        where TResponse : notnull
        where TRequest : notnull
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>Initializes a new instance of the <see cref="TransactionBehavior{TRequest, TResponse}"/> class.</summary>
        /// <param name="unitOfWork">The unit of work for transaction management.</param>
        /// <param name="logger">The logger instance.</param>
        public TransactionBehavior(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <inheritdoc />
        public override async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var response = await next();

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return response;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Template.Application.Common.Results;

using Zentient.Results;

namespace Template.Application.Common.Behaviors
{
    /// <summary>
    /// Pipeline behavior that performs validation using all registered <see cref="IValidator{TRequest}"/> instances for the request.
    /// Throws a <see cref="ValidationException"/> if validation fails.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request to validate.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public sealed class ValidationBehavior<TRequest, TResponse>
        : PipelineBehaviorBase<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        /// <summary>Initializes a new instance of the <see cref="ValidationBehavior{TRequest, TResponse}"/> class.</summary>
        /// <param name="validators">The validators to use for the request.</param>
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        /// <summary>
        /// Handles the validation logic for the request. If validation fails, throws a <see cref="ValidationException"/>.
        /// Otherwise, calls the next handler in the pipeline.
        /// </summary>
        /// <param name="request">The request instance.</param>
        /// <param name="next">The next handler in the pipeline.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The response from the next handler if validation succeeds.</returns>
        /// <exception cref="ValidationException">Thrown if validation fails.</exception>
        public override async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);
            var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            List<ValidationFailure> failures = results.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Count != 0)
            {
                // TODO: Fix IValidator<TRequest> to return a Result instead of throwing an exception
                if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(IResult<>))
                {
                    var validationResult = Result.Failure<TResponse>(
                        AppData.Validation.ValidationFailed(failures),
                        ResultStatuses.BadRequest);
                    return (TResponse)(object)validationResult;
                }

                throw new ValidationException(failures);
            }

            return await next();
        }
    }
}

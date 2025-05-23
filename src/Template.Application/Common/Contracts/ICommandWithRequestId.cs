using MediatR;

using Template.Domain.Common.Result;
using Template.Domain.ValueObjects;

using Zentient.Results;

namespace Template.Application.Common.Contracts
{
    public interface ICommandWithRequestId<TResponse> : IRequest<TResponse>
        where TResponse : IResult
    {
        /// <summary>
        /// Gets the request identifier.
        /// </summary>
        RequestId ClientRequestId { get; }
    }
}

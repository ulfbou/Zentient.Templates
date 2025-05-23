
using Template.Domain.Common.Result;
using Template.Domain.Entities;

using Zentient.Results;

namespace Template.Application.Common.Contracts
{
    public interface IUserRepository
    {
        Task AddAsync(TenantUser adminUser, bool commit, CancellationToken cancellationToken);
        Task<IResult> ExistsByEmailAsync(object adminEmail, CancellationToken cancellationToken);
    }
}
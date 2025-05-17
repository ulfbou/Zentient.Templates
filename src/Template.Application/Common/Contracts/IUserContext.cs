using Template.Domain.ValueObjects;

namespace Template.Application.Common.Contracts
{
    public interface IUserContext
    {
        UserId UserId { get; }
        TenantId TenantId { get; }
        IReadOnlyCollection<string> Roles { get; }
    }
}

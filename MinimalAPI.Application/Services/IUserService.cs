using Zentient.Templates.MinimalAPI.Domain.Entities;

namespace Zentient.Templates.MinimalAPI.Application.Services
{
    public interface IUserService
    {
        Task<int> CreateAsync(User user, CancellationToken ct);
        Task<int> DeleteAsync(Guid id, CancellationToken ct);
        Task<IEnumerable<User>> GetAsync(Func<User, bool> value, CancellationToken ct);
        Task<User?> GetAsync(Guid id, CancellationToken ct);
        Task<int> UpdateAsync(Guid id, User user, CancellationToken ct);
    }
}

using System.Collections.Concurrent;

using Zentient.Templates.MinimalAPI.Application.Services;
using Zentient.Templates.MinimalAPI.Domain.Entities;

namespace Zentient.Templates.MinimalAPI.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly ConcurrentDictionary<Guid, User> _users = new();

        public Task<IEnumerable<User>> GetAsync(Func<User, bool> predicate, CancellationToken ct)
        {
            IEnumerable<User> users = _users.Values.Where(predicate);
            return Task.FromResult(users);
        }
        public Task<User?> GetAsync(Guid id, CancellationToken ct)
        {
            if (_users.TryGetValue(id, out User? user))
            {
                return Task.FromResult<User?>(user);
            }
            return Task.FromResult<User?>(null);
        }
        public Task<int> CreateAsync(User user, CancellationToken ct)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.Id = Guid.NewGuid();
            _users.TryAdd(user.Id, user);
            return Task.FromResult(201);
        }
        public Task<int> UpdateAsync(Guid id, User user, CancellationToken ct)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (_users.TryGetValue(id, out User? existingUser))
            {
                user.Id = id;
                _users[id] = user;
                return Task.FromResult(200);
            }
            return Task.FromResult(404);
        }

        public Task<int> DeleteAsync(Guid id, CancellationToken ct)
        {
            if (_users.TryRemove(id, out _))
            {
                return Task.FromResult(200);
            }
            return Task.FromResult(404);
        }
    }
}

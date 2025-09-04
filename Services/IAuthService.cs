using Skii.Models;

namespace Skii.Services;

public interface IAuthService
{
    Task<User> AddUser(User user, CancellationToken cancellationToken = default);
    Task<bool> CheckUserExists(string googleId, CancellationToken cancellationToken = default);
    Task<List<User>> GetUsers(CancellationToken cancellationToken = default);
}
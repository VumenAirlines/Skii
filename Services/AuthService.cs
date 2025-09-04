using Microsoft.EntityFrameworkCore;
using Skii.Data;
using Skii.Models;

namespace Skii.Services;

public class AuthService(AppDbContext dbContext):IAuthService
{
    public async Task<bool> CheckUserExists(string googleId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.GoogleId == googleId,cancellationToken) is { } user;
    }
    public async Task<User> AddUser(User user,CancellationToken cancellationToken = default)
    {
        if (await CheckUserExists(user.GoogleId, cancellationToken))
            return user;
        await dbContext.AddAsync(user,cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<List<User>> GetUsers(CancellationToken cancellationToken=default)
    {
        return await dbContext.Users.ToListAsync(cancellationToken);
    }

}
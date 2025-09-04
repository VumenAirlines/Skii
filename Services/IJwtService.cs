using Skii.Models;

namespace Skii.Services;

public interface IJwtService
{
    
    string GenerateJwtToken(string email, string googleId, Guid userId);
}
using System.Security.Claims;

namespace TaskManagement.Api.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(IEnumerable<Claim> claims);
    }
}

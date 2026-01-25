using E_commarce_Backend.Models;

namespace E_commarce_Backend.Services.Abstractions
{
    public interface IJwtService
    {
        Task<string> GenerateToken(AppUser user);
    }
}

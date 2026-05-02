using E_commarce_Backend.Dtos.Profile;

namespace E_commarce_Backend.Services.Abstractions
{
    public interface IUserService
    {
        Task<UserProfileDto> GetProfileAsync(string userId);

        Task UpdateProfileAsync(string userId, UpdateProfileDto dto);
    }
}

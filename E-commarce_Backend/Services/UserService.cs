namespace E_commarce_Backend.Services
{
    using E_commarce_Backend.Dtos.Profile;
    using E_commarce_Backend.Models.User;
    using E_commarce_Backend.Services.Abstractions;
    using Microsoft.AspNetCore.Identity;

    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> userManager;

        public UserService(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<UserProfileDto> GetProfileAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                throw new Exception("User not found");

            return new UserProfileDto
            {
                Email = user.Email!,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                //Address = user.Address
            };
        }

        public async Task UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                throw new Exception("User not found");

            user.FullName = dto.FullName ?? user.FullName;
            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
            //user.Address = dto.Address ?? user.Address;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new Exception("Failed to update profile");
        }
    }
}

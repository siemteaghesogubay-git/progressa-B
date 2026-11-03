using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using progressa.Data;
using progressa.Services.IServices;

namespace progressa.Services
{
    public class UserService(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext) : IUserService
    {
        public async Task<DeleteUserResult> DeleteAccountAsync(string userId, string password)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return DeleteUserResult.NotFound();
            }

            var passwordIsCorrect = await userManager.CheckPasswordAsync(user, password);

            if (!passwordIsCorrect)
            {
                return DeleteUserResult.InvalidPassword();
            }
            
            await dbContext.Goals.Where(g => g.OwnerUserId == userId).ExecuteDeleteAsync();

            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return DeleteUserResult.Failed(
                    result.Errors.Select(e => e.Description));
            }

            return DeleteUserResult.Ok();
        }

    }
}
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using progressa.Data;
using progressa.Services.IServices;

namespace progressa.Services;

public class UserService(
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext dbContext) : IUserService
{
    public async Task<DeleteUserResult> DeleteAccountAsync(
        string userId,
        string password)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return DeleteUserResult.NotFound();
        }

        var passwordIsCorrect = await userManager.CheckPasswordAsync(
            user,
            password);

        if (!passwordIsCorrect)
        {
            return DeleteUserResult.InvalidPassword();
        }

        await using var transaction =
            await dbContext.Database.BeginTransactionAsync();

        await dbContext.Notifications
            .Where(notification => notification.UserId == userId)
            .ExecuteDeleteAsync();

        await dbContext.ActivitySessions
            .Where(session => session.OwnerUserId == userId)
            .ExecuteDeleteAsync();

        await dbContext.RecurringWorkoutSchedules
            .Where(schedule => schedule.OwnerUserId == userId)
            .ExecuteDeleteAsync();

        await dbContext.Challenges
            .Where(challenge =>
                challenge.UserId == userId ||
                challenge.UserOwnerId == userId)
            .ExecuteDeleteAsync();

        await dbContext.Categories
            .Where(category => category.OwnerUserId == userId)
            .ExecuteDeleteAsync();

        await dbContext.Goals
            .Where(goal => goal.OwnerUserId == userId)
            .ExecuteDeleteAsync();

        await dbContext.Calendars
            .Where(calendar => calendar.OwnerUserId == userId)
            .ExecuteDeleteAsync();

        await dbContext.NutritionLogs
            .Where(log => log.UserId == userId)
            .ExecuteDeleteAsync();

        await dbContext.SleepLogs
            .Where(log => log.UserId == userId)
            .ExecuteDeleteAsync();

        await dbContext.UserSettings
            .Where(settings => settings.UserId == userId)
            .ExecuteDeleteAsync();

        var result = await userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            await transaction.RollbackAsync();

            return DeleteUserResult.Failed(
                result.Errors.Select(error => error.Description));
        }

        await transaction.CommitAsync();

        return DeleteUserResult.Ok();
    }
}
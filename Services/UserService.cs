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

            // Hård radering: all data som hör till användaren tas bort permanent.
            //
            // Nästan alla relationer till ApplicationUser har DeleteBehavior.Restrict, så
            // databasen raderar inget automatiskt och vägrar ta bort en rad som något annat
            // fortfarande pekar på. Därför raderar vi i den här ordningen: det som pekar på
            // något tas bort före det som det pekar på.
            //
            //   Notifications    -> ActivitySessions
            //   ActivitySessions -> Calendars, Categories
            //   Challenges       -> Goals, Categories
            //   Goals            -> Calendars
            //   allt             -> användaren
            //
            // Lägger du till en tabell som pekar på användaren (eller på något i listan nedan)
            // måste den raderas här också, på rätt plats. Annars stoppar databasen raderingen
            // med ett FK-fel.
            //
            // Allt sker i en transaktion: antingen raderas allt eller ingenting. Om något
            // kastar ett undantag rullas transaktionen tillbaka när den disposas (await using).
            // userManager använder samma DbContext och ingår därför i transaktionen.

            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            await dbContext.Notifications.Where(n => n.UserId == userId).ExecuteDeleteAsync();
            await dbContext.ActivitySessions.Where(a => a.OwnerUserId == userId).ExecuteDeleteAsync();

            // Båda kolumnerna kontrolleras: användaren kan ha skapat utmaningen (UserOwnerId)
            // eller ha den (UserId).
            await dbContext.Challenges.Where(c => c.UserId == userId || c.UserOwnerId == userId).ExecuteDeleteAsync();

            await dbContext.Categories.Where(c => c.OwnerUserId == userId).ExecuteDeleteAsync();
            await dbContext.Goals.Where(g => g.OwnerUserId == userId).ExecuteDeleteAsync();
            await dbContext.Calendars.Where(c => c.OwnerUserId == userId).ExecuteDeleteAsync();
            await dbContext.NutritionLogs.Where(n => n.UserId == userId).ExecuteDeleteAsync();
            await dbContext.SleepLogs.Where(s => s.UserId == userId).ExecuteDeleteAsync();
            await dbContext.UserSettings.Where(u => u.UserId == userId).ExecuteDeleteAsync();

            // Identitys egna tabeller (roller, claims, inloggningar, tokens) har Cascade
            // och försvinner automatiskt med användaren.
            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                await transaction.RollbackAsync();
                return DeleteUserResult.Failed(
                    result.Errors.Select(e => e.Description));
            }

            await transaction.CommitAsync();
            return DeleteUserResult.Ok();
        }

    }
}
using Microsoft.EntityFrameworkCore;
using progressa.Data;
using progressa.Repositories.interfaces;

namespace progressa.Repositories;

public class GoalRepository(ApplicationDbContext db) : IGoalRepository
{
    public Task<List<Goal>> GetAllAsync(
        string userId,
        CancellationToken ct = default)
    {
        return db.Goals
            .AsNoTracking()
            .Where(g => g.OwnerUserId == userId)
            .OrderBy(g => g.Id)
            .ToListAsync(ct);
    }

    public Task<Goal?> GetByIdAsync(
        int id,
        string userId,
        CancellationToken ct = default)
    {
        return db.Goals
            .AsNoTracking()
            .FirstOrDefaultAsync(
                g => g.Id == id && g.OwnerUserId == userId,
                ct);
    }

    public Task<Goal?> GetForUpdateAsync(
        int id,
        string userId,
        CancellationToken ct = default)
    {
        return db.Goals
            .FirstOrDefaultAsync(
                g => g.Id == id && g.OwnerUserId == userId,
                ct);
    }

    public Task<bool> HasChallengesAsync(
        int id,
        string userId,
        CancellationToken ct = default)
    {
        return db.Challenges
            .AnyAsync(
                c => c.GoalId == id &&
                     c.Goal.OwnerUserId == userId,
                ct);
    }

    public async Task AddAsync(
        Goal goal,
        CancellationToken ct = default)
    {
        db.Goals.Add(goal);
        await db.SaveChangesAsync(ct);
    }

    public async Task SaveChangesAsync(
        CancellationToken ct = default)
    {
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(
        Goal goal,
        CancellationToken ct = default)
    {
        db.Goals.Remove(goal);
        await db.SaveChangesAsync(ct);
    }
}
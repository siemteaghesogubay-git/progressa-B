using Microsoft.EntityFrameworkCore;
using progressa.Data;
using progressa.Repositories.interfaces;

namespace progressa.Repositories;

public class CategoryRepository(ApplicationDbContext db)
    : ICategoryRepository
{
    public Task<List<Category>> GetAllAsync(
        string userId,
        CancellationToken ct = default)
    {
        return db.Categories
            .AsNoTracking()
            .Where(c => c.OwnerUserId == userId)
            .OrderBy(c => c.Name)
            .ThenBy(c => c.Id)
            .ToListAsync(ct);
    }

    public Task<Category?> GetByIdAsync(
        int id,
        string userId,
        CancellationToken ct = default)
    {
        return db.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(
                c => c.Id == id && c.OwnerUserId == userId,
                ct);
    }
}
using progressa.Models;

namespace progressa.Repositories.interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync(
        string userId,
        CancellationToken ct = default);

    Task<Category?> GetByIdAsync(
        int id,
        string userId,
        CancellationToken ct = default);

}
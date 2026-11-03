namespace progressa.Repositories.interfaces;

public interface ICalendarRepository
{
    Task<bool> ExistsAsync(
        int id,
        string userId,
        CancellationToken ct = default);

}
using Microsoft.EntityFrameworkCore;
using progressa.Data;
using progressa.Services.IServices;
using progressa.Dtos;

namespace progressa.Services
{
    public class GoalService : IGoalService
    {
        private readonly ApplicationDbContext _context;

        public GoalService(ApplicationDbContext context)
        {
            _context = context;
        }


        // Hämta alla mål för en användare
        public async Task<List<GoalDto>> GetAllAsync(string userId)
        {
            return await _context.Goals
                .Where(g => g.OwnerUserId == userId)
                .Select(g => new GoalDto
                {
                    Id = g.Id,
                    Title = g.Title,
                    Target = g.Target,
                    StartDate = g.StartDate,
                    EndDate = g.EndDate,
                    IsCompleted = g.IsCompleted,
                    CalendarId = g.CalendarId
                })
                .ToListAsync();
        }

        // Hämta ett specifikt mål för en användare
        public async Task<GoalDto?> GetByIdAsync(int id, string userId){
            return await _context.Goals
                .Where(g => g.Id == id && g.OwnerUserId == userId)
                .Select(g => new GoalDto
                {
                    Id = g.Id,
                    Title = g.Title,
                    Target = g.Target,
                    StartDate = g.StartDate,
                    EndDate = g.EndDate,
                    IsCompleted = g.IsCompleted,
                    CalendarId = g.CalendarId
                })
                .FirstOrDefaultAsync();
        }

        // Skapa ett nytt mål för en användare
        public async Task<GoalDto> CreateAsync(CreateGoalDto dto, string userId)
        {
            if (dto.StartDate == default || dto.EndDate == default ||
                dto.EndDate < dto.StartDate)
            {
                throw new ArgumentException("Ogiltiga datum.");
            }

            if (dto.CalendarId.HasValue)
            {
                bool calendarExists = await _context.Calendars
                    .AnyAsync(c => c.Id == dto.CalendarId.Value &&
                                   c.OwnerUserId == userId);

                if (!calendarExists)
                {
                    throw new ArgumentException("Kalendern finns inte.");
                }
            }

            var goal = new Goal
            {
                Title = dto.Title,
                Target = dto.Target,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsCompleted = false,
                OwnerUserId = userId,
                CalendarId = dto.CalendarId
            };

            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();

            return new GoalDto
            {
                Id = goal.Id,
                Title = goal.Title,
                Target = goal.Target,
                StartDate = goal.StartDate,
                EndDate = goal.EndDate,
                IsCompleted = goal.IsCompleted,
                CalendarId = goal.CalendarId
            };
        }

        // Uppdatera ett mål för en användare
        public async Task<GoalDto?> UpdateAsync(int id, UpdateGoalDto dto, string userId)
        {
            var goal = await _context.Goals
                .FirstOrDefaultAsync(g => g.Id == id && g.OwnerUserId == userId);

            if (goal == null)
                return null;

            if (dto.StartDate == default || dto.EndDate == default ||
                dto.EndDate < dto.StartDate)
            {
                throw new ArgumentException("Ogiltiga datum.");
            }

            if (dto.CalendarId.HasValue)
            {
                bool calendarExists = await _context.Calendars
                    .AnyAsync(c => c.Id == dto.CalendarId.Value &&
                                   c.OwnerUserId == userId);

                if (!calendarExists)
                    throw new ArgumentException("Kalendern finns inte.");
            }

            goal.Title = dto.Title;
            goal.Target = dto.Target;
            goal.StartDate = dto.StartDate;
            goal.EndDate = dto.EndDate;
            goal.IsCompleted = dto.IsCompleted;
            goal.CalendarId = dto.CalendarId;

            await _context.SaveChangesAsync();

            return new GoalDto
            {
                Id = goal.Id,
                Title = goal.Title,
                Target = goal.Target,
                StartDate = goal.StartDate,
                EndDate = goal.EndDate,
                IsCompleted = goal.IsCompleted,
                CalendarId = goal.CalendarId
            };
        }

        // Ta bort ett mål för en användare
        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var goal = await _context.Goals
                .FirstOrDefaultAsync(g => g.Id == id && g.OwnerUserId == userId);

            if (goal == null)
                return false;

            bool hasChallenges = await _context.Challenges
                .AnyAsync(c => c.GoalId == id);

            if (hasChallenges)
                throw new InvalidOperationException("Målet används av en challenge och kan inte tas bort.");

            _context.Goals.Remove(goal);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

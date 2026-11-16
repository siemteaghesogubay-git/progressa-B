namespace progressa.Dtos
{
    public class CalendarSessionDto
    {
        public int Id { get; set; }
        public int CalendarId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public int PlannedDurationInMinutes { get; set; }
        public int? ActualDurationInMinutes { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

namespace progressa.Dtos
{
    public class GoalDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Target { get; set; }
        public GoalMetric? Metric { get; set; }
        public int? CategoryId { get; set; }
        public int? Progress { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCompleted { get; set; }
        public int? CalendarId { get; set; }
    }
}

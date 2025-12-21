namespace EnvironmentsService.Application.Events
{
    public class EnvironmentDeletedEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
        public string EventType { get; set; } = "EnvironmentDeleted";

        public Guid EnvironmentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid DeletedBy { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}
namespace EnvironmentsService.Application.Events
{
    public class EnvironmentUpdatedEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
        public string EventType { get; set; } = "EnvironmentUpdated";

        public Guid EnvironmentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
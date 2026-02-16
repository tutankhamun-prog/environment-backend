namespace EnvironmentsService.Application.Events
{
    public class EnvironmentUserRemovedEvent
    {
        public Guid EnvironmentId { get; set; }
        public Guid UserId { get; set; }
        public Guid RemovedBy { get; set; }
        public DateTime RemovedAt { get; set; }
    }
}
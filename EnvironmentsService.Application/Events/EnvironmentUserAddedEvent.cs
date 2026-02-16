namespace EnvironmentsService.Application.Events
{
    public class EnvironmentUserAddedEvent
    {
        public Guid EnvironmentId { get; set; }
        public Guid UserId { get; set; }
        public Guid AddedBy { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
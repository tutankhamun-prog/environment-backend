namespace EnvironmentsService.Application.DTOs.Responses
{
    public class EnvironmentMemberResponseDto
    {
        public Guid Id { get; set; }
        public Guid EnvironmentId { get; set; }
        public Guid UserId { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime AddedAt { get; set; }
        public Guid AddedBy { get; set; }
    }
}
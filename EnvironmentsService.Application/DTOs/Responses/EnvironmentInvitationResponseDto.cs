namespace EnvironmentsService.Application.DTOs.Responses
{
    public class EnvironmentInvitationResponseDto
    {
        public Guid EnvironmentId { get; set; }
        public string EnvironmentName { get; set; } = string.Empty;
        public string? EnvironmentDescription { get; set; }
        public Guid InvitedBy { get; set; }
        public DateTime InvitedAt { get; set; }
        public string Role { get; set; } = "Member";
    }
}
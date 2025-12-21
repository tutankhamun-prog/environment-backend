namespace EnvironmentsService.Application.DTOs.Responses
{
    public class EnvironmentResponseDto
    {
        // Ce qu'on retourne à l'utilisateur
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CreatedBy { get; set; }  // UserId du chef de projet
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CurrentUserRole { get; set; } // "ProjectManager" ou "Member"            

    }
}
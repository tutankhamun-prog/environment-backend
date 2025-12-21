namespace EnvironmentsService.Application.DTOs.Requests
{
    public class AddMemberRequestDto
    {
        // ID de l'utilisateur à ajouter
        public Guid UserId { get; set; }

        // Rôle (optionnel, "Member" par défaut)
        public string Role { get; set; } = "Member";
    }
}
namespace EnvironmentsService.Domain.Entities
{
    public class EnvironmentMember
    {
        // ID de la relation
        public Guid Id { get; set; } = Guid.NewGuid();

        // ID de l'environnement
        public Guid EnvironmentId { get; set; }

        // ID du membre (UserId)
        public Guid UserId { get; set; }

        // Rôle du membre dans l'environnement (pour extension future)
        // "Member" par défaut, pourrait être "Admin", "ReadOnly", etc.
        public string Role { get; set; } = "Member";

        // Quand le membre a été ajouté
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Qui a ajouté ce membre (généralement le chef de projet)
        public Guid AddedBy { get; set; }

        // Navigation property vers l'environnement
        public Environment? Environment { get; set; }
    }
}
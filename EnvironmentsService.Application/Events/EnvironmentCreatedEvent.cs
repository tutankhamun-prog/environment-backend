namespace EnvironmentsService.Application.Events
{
    public class EnvironmentCreatedEvent
    {
        // Identifiant de l'événement (pour traçabilité)
        public Guid EventId { get; set; } = Guid.NewGuid();

        // Quand l'événement s'est produit
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

        // Type d'événement
        public string EventType { get; set; } = "EnvironmentCreated";

        // Données de l'environnement créé
        public Guid EnvironmentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CreatedBy { get; set; }  // Le chef de projet
        public DateTime CreatedAt { get; set; }
    }
}
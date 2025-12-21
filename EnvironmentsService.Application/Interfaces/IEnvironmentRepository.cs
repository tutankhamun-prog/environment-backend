using Environment = EnvironmentsService.Domain.Entities.Environment;


namespace EnvironmentsService.Application.Interfaces
{
    public interface IEnvironmentRepository
    {
        // Récupérer UN environnement par son ID
        // Task<T> = opération asynchrone qui retourne T
        // ? après Environment = peut retourner null si pas trouvé
        Task<Environment?> GetByIdAsync(Guid id);

        // Récupérer TOUS les environnements actifs
        // IEnumerable = collection d'éléments
        Task<IEnumerable<Environment>> GetAllAsync();

        // Récupérer tous les environnements créés par un utilisateur spécifique
        // Utile pour afficher "Mes environnements"
        Task<IEnumerable<Environment>> GetByCreatedByAsync(Guid userId);

        // Créer un nouvel environnement
        // Retourne l'environnement créé (avec son ID généré)
        Task<Environment> CreateAsync(Environment environment);

        // Mettre à jour un environnement existant
        Task<Environment> UpdateAsync(Environment environment);

        // Supprimer un environnement (soft delete)
        // bool = true si supprimé, false si pas trouvé
        Task<bool> DeleteAsync(Guid id);

        // Vérifier si un environnement existe
        // Utile avant de faire une opération
        Task<bool> ExistsAsync(Guid id);
    }
}
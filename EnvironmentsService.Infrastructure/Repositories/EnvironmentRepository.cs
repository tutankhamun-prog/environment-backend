using Microsoft.EntityFrameworkCore;
using EnvironmentsService.Application.Interfaces;
using EnvironmentsService.Domain.Entities;
using EnvironmentsService.Infrastructure.Data.Contexts;
using Environment = EnvironmentsService.Domain.Entities.Environment;  // ← AJOUT


namespace EnvironmentsService.Infrastructure.Repositories
{
    public class EnvironmentRepository : IEnvironmentRepository
    {
        // Le DbContext = la connexion à la base de données
        private readonly ApplicationDbContext _context;

        // Injection de dépendance : ASP.NET Core va nous fournir le DbContext
        public EnvironmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // MÉTHODE 1 : Récupérer un environnement par ID
        public async Task<Environment?> GetByIdAsync(Guid id)
        {
            // FirstOrDefaultAsync = retourne le premier élément qui correspond, ou null
            // On filtre par ID ET IsActive (on ne retourne pas les supprimés)
            return await _context.Environments
                .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);
        }

        // MÉTHODE 2 : Récupérer tous les environnements
        public async Task<IEnumerable<Environment>> GetAllAsync()
        {
            return await _context.Environments
                .Where(e => e.IsActive)  // Seulement les actifs
                .OrderByDescending(e => e.CreatedAt)  // Les plus récents d'abord
                .ToListAsync();  // Exécute la requête et retourne une liste
        }

        // MÉTHODE 3 : Récupérer les environnements d'un utilisateur
        public async Task<IEnumerable<Environment>> GetByCreatedByAsync(Guid userId)
        {
            return await _context.Environments
                .Where(e => e.CreatedBy == userId && e.IsActive)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        // MÉTHODE 4 : Créer un nouvel environnement
        public async Task<Environment> CreateAsync(Environment environment)
        {
            // AddAsync = ajoute l'entité au contexte (en mémoire)
            await _context.Environments.AddAsync(environment);

            // SaveChangesAsync = exécute réellement l'INSERT en base
            await _context.SaveChangesAsync();

            // On retourne l'environnement avec son ID généré
            return environment;
        }

        // MÉTHODE 5 : Mettre à jour un environnement
        public async Task<Environment> UpdateAsync(Environment environment)
        {
            // On met à jour la date de modification
            environment.UpdatedAt = DateTime.UtcNow;

            // Update = marque l'entité comme modifiée
            _context.Environments.Update(environment);

            // SaveChangesAsync = exécute l'UPDATE en base
            await _context.SaveChangesAsync();

            return environment;
        }

        // MÉTHODE 6 : Supprimer un environnement (SOFT DELETE)
        public async Task<bool> DeleteAsync(Guid id)
        {
            // On cherche l'environnement
            var environment = await _context.Environments.FindAsync(id);

            // S'il n'existe pas, on retourne false
            if (environment == null) return false;

            // SOFT DELETE : on ne supprime pas vraiment, on met IsActive = false
            // Pourquoi ? Pour garder l'historique et pouvoir restaurer
            environment.IsActive = false;
            environment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        // MÉTHODE 7 : Vérifier si un environnement existe
        public async Task<bool> ExistsAsync(Guid id)
        {
            // AnyAsync = retourne true si au moins 1 élément correspond
            return await _context.Environments
                .AnyAsync(e => e.Id == id && e.IsActive);
        }
    }
}
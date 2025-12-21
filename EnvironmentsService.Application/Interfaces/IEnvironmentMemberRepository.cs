using EnvironmentsService.Domain.Entities;

namespace EnvironmentsService.Application.Interfaces
{
    public interface IEnvironmentMemberRepository
    {
        // Méthodes existantes
        Task<EnvironmentMember> AddMemberAsync(EnvironmentMember member);
        Task<bool> RemoveMemberAsync(Guid environmentId, Guid userId);
        Task<IEnumerable<EnvironmentMember>> GetMembersByEnvironmentIdAsync(Guid environmentId);
        Task<IEnumerable<Guid>> GetEnvironmentIdsByUserIdAsync(Guid userId);
        Task<bool> IsMemberAsync(Guid environmentId, Guid userId);

        // ✅ NOUVELLES MÉTHODES
        Task<IEnumerable<EnvironmentMember>> GetByUserIdAsync(Guid userId);
        Task<EnvironmentMember?> GetByEnvironmentAndUserAsync(Guid environmentId, Guid userId);
    }
}
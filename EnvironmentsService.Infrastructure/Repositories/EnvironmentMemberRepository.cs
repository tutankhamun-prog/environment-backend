using Microsoft.EntityFrameworkCore;
using EnvironmentsService.Application.Interfaces;
using EnvironmentsService.Domain.Entities;
using EnvironmentsService.Infrastructure.Data.Contexts;

namespace EnvironmentsService.Infrastructure.Repositories
{
    public class EnvironmentMemberRepository : IEnvironmentMemberRepository
    {
        private readonly ApplicationDbContext _context;

        public EnvironmentMemberRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EnvironmentMember> AddMemberAsync(EnvironmentMember member)
        {
            await _context.EnvironmentMembers.AddAsync(member);
            await _context.SaveChangesAsync();
            return member;
        }

        public async Task<bool> RemoveMemberAsync(Guid environmentId, Guid userId)
        {
            var member = await _context.EnvironmentMembers
                .FirstOrDefaultAsync(em => em.EnvironmentId == environmentId && em.UserId == userId);

            if (member == null)
                return false;

            _context.EnvironmentMembers.Remove(member);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<EnvironmentMember>> GetMembersByEnvironmentIdAsync(Guid environmentId)
        {
            return await _context.EnvironmentMembers
                .Where(em => em.EnvironmentId == environmentId)
                .OrderBy(em => em.AddedAt)
                .ToListAsync();
        }

        public async Task<bool> IsMemberAsync(Guid environmentId, Guid userId)
        {
            return await _context.EnvironmentMembers
                .AnyAsync(em => em.EnvironmentId == environmentId && em.UserId == userId);
        }

        public async Task<IEnumerable<Guid>> GetEnvironmentIdsByUserIdAsync(Guid userId)
        {
            return await _context.EnvironmentMembers
                .Where(em => em.UserId == userId)
                .Select(em => em.EnvironmentId)
                .ToListAsync();
        }

        // ✅ Ajoutez ces méthodes à la fin de la classe EnvironmentMemberRepository

        public async Task<IEnumerable<EnvironmentMember>> GetByUserIdAsync(Guid userId)
        {
            return await _context.EnvironmentMembers
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.AddedAt)
                .ToListAsync();
        }

        public async Task<EnvironmentMember?> GetByEnvironmentAndUserAsync(Guid environmentId, Guid userId)
        {
            return await _context.EnvironmentMembers
                .FirstOrDefaultAsync(m => m.EnvironmentId == environmentId && m.UserId == userId);
        }
    }
}
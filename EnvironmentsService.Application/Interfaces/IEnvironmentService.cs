using EnvironmentsService.Application.DTOs.Requests;
using EnvironmentsService.Application.DTOs.Responses;

namespace EnvironmentsService.Application.Interfaces
{
    public interface IEnvironmentService
    {
        Task<EnvironmentResponseDto> CreateEnvironmentAsync(CreateEnvironmentRequestDto request, Guid userId);
        Task<EnvironmentResponseDto?> GetEnvironmentByIdAsync(Guid id);
        Task<IEnumerable<EnvironmentResponseDto>> GetAllEnvironmentsAsync();
        Task<IEnumerable<EnvironmentResponseDto>> GetMyEnvironmentsAsync(Guid userId);
        Task<EnvironmentResponseDto> UpdateEnvironmentAsync(Guid id, UpdateEnvironmentRequestDto request, Guid userId);
        Task<bool> DeleteEnvironmentAsync(Guid id, Guid userId);
        
        // Gestion des membres
        Task<EnvironmentMemberResponseDto> AddMemberAsync(Guid environmentId, AddMemberRequestDto request, Guid currentUserId);
        Task<bool> RemoveMemberAsync(Guid environmentId, Guid userId, Guid currentUserId);
        Task<IEnumerable<EnvironmentMemberResponseDto>> GetMembersAsync(Guid environmentId);
        Task<IEnumerable<EnvironmentResponseDto>> GetAccessibleEnvironmentsAsync(Guid userId);
        // Voir les invitations en attente
        Task<IEnumerable<EnvironmentInvitationResponseDto>> GetPendingInvitationsAsync(Guid userId);

        // Accepter une invitation
        Task<EnvironmentMemberResponseDto> AcceptInvitationAsync(Guid environmentId, Guid userId);
    }
}
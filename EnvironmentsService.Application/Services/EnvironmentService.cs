using AutoMapper;
using Microsoft.Extensions.Configuration;
using EnvironmentsService.Application.DTOs.Requests;
using EnvironmentsService.Application.DTOs.Responses;
using EnvironmentsService.Application.Interfaces;
using EnvironmentsService.Application.Events;
using EnvironmentsService.Domain.Entities;
using Environment = EnvironmentsService.Domain.Entities.Environment;

namespace EnvironmentsService.Application.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        private readonly IEnvironmentRepository _repository;
        private readonly IEnvironmentMemberRepository _memberRepository;
        private readonly IMapper _mapper;
        private readonly IKafkaProducer _kafkaProducer;
        private readonly IConfiguration _configuration;

        public EnvironmentService(
            IEnvironmentRepository repository,
            IEnvironmentMemberRepository memberRepository,
            IMapper mapper,
            IKafkaProducer kafkaProducer,
            IConfiguration configuration)
        {
            _repository = repository;
            _memberRepository = memberRepository;
            _mapper = mapper;
            _kafkaProducer = kafkaProducer;
            _configuration = configuration;
        }

        public async Task<EnvironmentResponseDto> CreateEnvironmentAsync(
            CreateEnvironmentRequestDto request,
            Guid userId)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Le nom de l'environnement est obligatoire");

            if (request.Name.Length > 200)
                throw new ArgumentException("Le nom ne peut pas dépasser 200 caractères");

            var environment = new Environment
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Description = request.Description?.Trim() ?? string.Empty,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var created = await _repository.CreateAsync(environment);

            var createdEvent = new EnvironmentCreatedEvent
            {
                EnvironmentId = created.Id,
                Name = created.Name,
                Description = created.Description,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.CreatedAt
            };

            var topic = _configuration["Kafka:Topics:EnvironmentCreated"] ?? "environment.created";
            await _kafkaProducer.ProduceAsync(topic, createdEvent);

            return _mapper.Map<EnvironmentResponseDto>(created);
        }

        public async Task<EnvironmentResponseDto?> GetEnvironmentByIdAsync(Guid id)
        {
            var environment = await _repository.GetByIdAsync(id);
            if (environment == null)
                return null;
            return _mapper.Map<EnvironmentResponseDto>(environment);
        }

        public async Task<IEnumerable<EnvironmentResponseDto>> GetAllEnvironmentsAsync()
        {
            var environments = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<EnvironmentResponseDto>>(environments);
        }

        public async Task<IEnumerable<EnvironmentResponseDto>> GetMyEnvironmentsAsync(Guid userId)
        {
            var environments = await _repository.GetByCreatedByAsync(userId);
            return _mapper.Map<IEnumerable<EnvironmentResponseDto>>(environments);
        }

        public async Task<EnvironmentResponseDto> UpdateEnvironmentAsync(
            Guid id,
            UpdateEnvironmentRequestDto request,
            Guid userId)
        {
            var environment = await _repository.GetByIdAsync(id);
            if (environment == null)
                throw new KeyNotFoundException($"Environnement {id} introuvable");

            if (environment.CreatedBy != userId)
                throw new UnauthorizedAccessException("Vous n'êtes pas autorisé à modifier cet environnement");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Le nom de l'environnement est obligatoire");

            environment.Name = request.Name.Trim();
            environment.Description = request.Description?.Trim() ?? string.Empty;
            environment.UpdatedAt = DateTime.UtcNow;

            var updated = await _repository.UpdateAsync(environment);

            var updatedEvent = new EnvironmentUpdatedEvent
            {
                EnvironmentId = updated.Id,
                Name = updated.Name,
                Description = updated.Description,
                UpdatedBy = userId,
                UpdatedAt = updated.UpdatedAt ?? DateTime.UtcNow
            };

            var topic = _configuration["Kafka:Topics:EnvironmentUpdated"] ?? "environment.updated";
            await _kafkaProducer.ProduceAsync(topic, updatedEvent);

            return _mapper.Map<EnvironmentResponseDto>(updated);
        }

        public async Task<bool> DeleteEnvironmentAsync(Guid id, Guid userId)
        {
            var environment = await _repository.GetByIdAsync(id);
            if (environment == null)
                return false;

            if (environment.CreatedBy != userId)
                throw new UnauthorizedAccessException("Vous n'êtes pas autorisé à supprimer cet environnement");

            var result = await _repository.DeleteAsync(id);

            if (result)
            {
                var deletedEvent = new EnvironmentDeletedEvent
                {
                    EnvironmentId = environment.Id,
                    Name = environment.Name,
                    DeletedBy = userId,
                    DeletedAt = DateTime.UtcNow
                };

                var topic = _configuration["Kafka:Topics:EnvironmentDeleted"] ?? "environment.deleted";
                await _kafkaProducer.ProduceAsync(topic, deletedEvent);
            }

            return result;
        }

        // ===== GESTION DES MEMBRES =====

        public async Task<EnvironmentMemberResponseDto> AddMemberAsync(
            Guid environmentId,
            AddMemberRequestDto request,
            Guid currentUserId)
        {
            var environment = await _repository.GetByIdAsync(environmentId);
            if (environment == null)
                throw new KeyNotFoundException($"Environnement {environmentId} introuvable");

            if (environment.CreatedBy != currentUserId)
                throw new UnauthorizedAccessException("Seul le chef de projet peut ajouter des membres");

            var alreadyMember = await _memberRepository.IsMemberAsync(environmentId, request.UserId);
            if (alreadyMember)
                throw new InvalidOperationException("Cet utilisateur est déjà membre de l'environnement");

            var member = new EnvironmentMember
            {
                EnvironmentId = environmentId,
                UserId = request.UserId,
                Role = request.Role,
                AddedBy = currentUserId,
                AddedAt = DateTime.UtcNow
            };

            var created = await _memberRepository.AddMemberAsync(member);

            return _mapper.Map<EnvironmentMemberResponseDto>(created);
        }

        public async Task<bool> RemoveMemberAsync(Guid environmentId, Guid userId, Guid currentUserId)
        {
            var environment = await _repository.GetByIdAsync(environmentId);
            if (environment == null)
                return false;

            if (environment.CreatedBy != currentUserId)
                throw new UnauthorizedAccessException("Seul le chef de projet peut retirer des membres");

            if (userId == currentUserId)
                throw new InvalidOperationException("Le chef de projet ne peut pas se retirer lui-même");

            var result = await _memberRepository.RemoveMemberAsync(environmentId, userId);

            return result;
        }

        public async Task<IEnumerable<EnvironmentMemberResponseDto>> GetMembersAsync(Guid environmentId)
        {
            var members = await _memberRepository.GetMembersByEnvironmentIdAsync(environmentId);
            return _mapper.Map<IEnumerable<EnvironmentMemberResponseDto>>(members);
        }

        public async Task<IEnumerable<EnvironmentResponseDto>> GetAccessibleEnvironmentsAsync(Guid userId)
        {
            var accessibleEnvironments = new List<EnvironmentResponseDto>();

            // 1. Environnements créés par l'utilisateur (Chef de projet)
            var createdEnvironments = await _repository.GetByCreatedByAsync(userId);
            foreach (var env in createdEnvironments)
            {
                var dto = _mapper.Map<EnvironmentResponseDto>(env);
                dto.CurrentUserRole = "ProjectManager"; // Chef de projet
                accessibleEnvironments.Add(dto);
            }

            // 2. Environnements où l'utilisateur est membre
            var memberships = await _memberRepository.GetByUserIdAsync(userId);
            foreach (var member in memberships)
            {
                var environment = await _repository.GetByIdAsync(member.EnvironmentId);
                if (environment != null && environment.IsActive)
                {
                    // Éviter les doublons (si déjà ajouté comme créateur)
                    if (!accessibleEnvironments.Any(e => e.Id == environment.Id))
                    {
                        var dto = _mapper.Map<EnvironmentResponseDto>(environment);
                        dto.CurrentUserRole = member.Role; // Role du membre
                        accessibleEnvironments.Add(dto);
                    }
                }
            }

            return accessibleEnvironments.OrderByDescending(e => e.CreatedAt);
        }

        // ===== GESTION DES INVITATIONS =====

        public async Task<IEnumerable<EnvironmentInvitationResponseDto>> GetPendingInvitationsAsync(Guid userId)
        {
            // Récupérer tous les membres où userId est présent
            var memberships = await _memberRepository.GetByUserIdAsync(userId);
            var invitations = new List<EnvironmentInvitationResponseDto>();

            foreach (var member in memberships)
            {
                var environment = await _repository.GetByIdAsync(member.EnvironmentId);
                if (environment != null && environment.IsActive)
                {
                    invitations.Add(new EnvironmentInvitationResponseDto
                    {
                        EnvironmentId = environment.Id,
                        EnvironmentName = environment.Name,
                        EnvironmentDescription = environment.Description,
                        InvitedBy = member.AddedBy,
                        InvitedAt = member.AddedAt,
                        Role = member.Role
                    });
                }
            }

            return invitations;
        }

        public async Task<EnvironmentMemberResponseDto> AcceptInvitationAsync(Guid environmentId, Guid userId)
        {
            // Vérifier que l'environnement existe
            var environment = await _repository.GetByIdAsync(environmentId);
            if (environment == null || !environment.IsActive)
                throw new KeyNotFoundException($"Environnement {environmentId} introuvable");

            // Vérifier que l'utilisateur est bien invité
            var member = await _memberRepository.GetByEnvironmentAndUserAsync(environmentId, userId);
            if (member == null)
                throw new UnauthorizedAccessException("Vous n'êtes pas invité à cet environnement");

            // L'invitation existe déjà, on retourne juste les infos
            return _mapper.Map<EnvironmentMemberResponseDto>(member);
        }
    }
}
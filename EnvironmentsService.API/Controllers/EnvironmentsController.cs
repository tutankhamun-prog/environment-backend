using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EnvironmentsService.Application.DTOs.Requests;
using EnvironmentsService.Application.DTOs.Responses;
using EnvironmentsService.Application.Interfaces;
using EnvironmentsService.API.Helpers;

namespace EnvironmentsService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // ✅ Tous les endpoints nécessitent un token JWT
    public class EnvironmentsController : ControllerBase
    {
        private readonly IEnvironmentService _environmentService;

        public EnvironmentsController(IEnvironmentService environmentService)
        {
            _environmentService = environmentService;
        }

        // POST /api/environments
        [HttpPost]
        [ProducesResponseType(typeof(EnvironmentResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateEnvironment([FromBody] CreateEnvironmentRequestDto request)
        {
            try
            {
                // ✅ Extraction du userId depuis le token JWT
                var userId = JwtHelper.GetUserIdFromClaims(User);
                var result = await _environmentService.CreateEnvironmentAsync(request, userId);
                return CreatedAtAction(nameof(GetEnvironmentById), new { id = result.Id }, result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue", details = ex.Message });
            }
        }

        // GET /api/environments/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EnvironmentResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetEnvironmentById(Guid id)
        {
            var result = await _environmentService.GetEnvironmentByIdAsync(id);
            if (result == null)
                return NotFound(new { message = $"Environnement {id} introuvable" });
            return Ok(result);
        }

        // GET /api/environments
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EnvironmentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllEnvironments()
        {
            var results = await _environmentService.GetAllEnvironmentsAsync();
            return Ok(results);
        }

        // GET /api/environments/my
        [HttpGet("my")]
        [ProducesResponseType(typeof(IEnumerable<EnvironmentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyEnvironments()
        {
            try
            {
                // ✅ Extraction du userId depuis le token JWT
                var userId = JwtHelper.GetUserIdFromClaims(User);
                var results = await _environmentService.GetMyEnvironmentsAsync(userId);
                return Ok(results);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        // PUT /api/environments/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EnvironmentResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateEnvironment(Guid id, [FromBody] UpdateEnvironmentRequestDto request)
        {
            try
            {
                // ✅ Extraction du userId depuis le token JWT
                var userId = JwtHelper.GetUserIdFromClaims(User);
                var result = await _environmentService.UpdateEnvironmentAsync(id, request, userId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue", details = ex.Message });
            }
        }

        // DELETE /api/environments/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEnvironment(Guid id)
        {
            try
            {
                // ✅ Extraction du userId depuis le token JWT
                var userId = JwtHelper.GetUserIdFromClaims(User);
                var result = await _environmentService.DeleteEnvironmentAsync(id, userId);
                if (!result)
                    return NotFound(new { message = $"Environnement {id} introuvable" });
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue", details = ex.Message });
            }
        }

        // ===== GESTION DES MEMBRES =====

        // POST /api/environments/{environmentId}/members
        [HttpPost("{environmentId}/members")]
        [ProducesResponseType(typeof(EnvironmentMemberResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddMember(Guid environmentId, [FromBody] AddMemberRequestDto request)
        {
            try
            {
                // ✅ Extraction du userId depuis le token JWT
                var userId = JwtHelper.GetUserIdFromClaims(User);
                var result = await _environmentService.AddMemberAsync(environmentId, request, userId);
                return CreatedAtAction(nameof(GetMembers), new { environmentId }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue", details = ex.Message });
            }
        }

        // DELETE /api/environments/{environmentId}/members/{userIdToRemove}
        [HttpDelete("{environmentId}/members/{userIdToRemove}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveMember(Guid environmentId, Guid userIdToRemove)
        {
            try
            {
                // ✅ Extraction du userId depuis le token JWT (celui qui fait l'action)
                var currentUserId = JwtHelper.GetUserIdFromClaims(User);
                var result = await _environmentService.RemoveMemberAsync(environmentId, userIdToRemove, currentUserId);

                if (!result)
                    return NotFound(new { message = "Membre introuvable" });

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue", details = ex.Message });
            }
        }

        // GET /api/environments/{environmentId}/members
        [HttpGet("{environmentId}/members")]
        [ProducesResponseType(typeof(IEnumerable<EnvironmentMemberResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMembers(Guid environmentId)
        {
            var results = await _environmentService.GetMembersAsync(environmentId);
            return Ok(results);
        }

        // GET /api/environments/accessible
        [HttpGet("accessible")]
        [ProducesResponseType(typeof(IEnumerable<EnvironmentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAccessibleEnvironments()
        {
            try
            {
                // ✅ Extraction du userId depuis le token JWT
                var userId = JwtHelper.GetUserIdFromClaims(User);
                var results = await _environmentService.GetAccessibleEnvironmentsAsync(userId);
                return Ok(results);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        // ===== GESTION DES INVITATIONS =====

        // GET /api/environments/invitations
        [HttpGet("invitations")]
        [ProducesResponseType(typeof(IEnumerable<EnvironmentInvitationResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPendingInvitations()
        {
            try
            {
                var userId = JwtHelper.GetUserIdFromClaims(User);
                var invitations = await _environmentService.GetPendingInvitationsAsync(userId);
                return Ok(invitations);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue", details = ex.Message });
            }
        }

        // POST /api/environments/{environmentId}/accept-invitation
        [HttpPost("{environmentId}/accept-invitation")]
        [ProducesResponseType(typeof(EnvironmentMemberResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AcceptInvitation(Guid environmentId)
        {
            try
            {
                var userId = JwtHelper.GetUserIdFromClaims(User);
                var result = await _environmentService.AcceptInvitationAsync(environmentId, userId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue", details = ex.Message });
            }
        }
    }
}
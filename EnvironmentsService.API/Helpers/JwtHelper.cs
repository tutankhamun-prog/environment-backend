using System.Security.Claims;

namespace EnvironmentsService.API.Helpers
{
    /// <summary>
    /// Classe helper pour extraire les informations du token JWT
    /// </summary>
    public static class JwtHelper
    {
        /// <summary>
        /// Extrait le userId depuis les claims du token JWT
        /// </summary>
        /// <param name="user">ClaimsPrincipal contenant les claims du token</param>
        /// <returns>Guid du userId</returns>
        /// <exception cref="UnauthorizedAccessException">Si le token est invalide ou le userId manquant</exception>
        public static Guid GetUserIdFromClaims(ClaimsPrincipal user)
        {
            // Cherche le claim "sub" (Subject) ou "NameIdentifier"
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)
                           ?? user.FindFirst("sub");

            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("Token invalide : userId manquant dans les claims");
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException($"Token invalide : userId '{userIdClaim.Value}' n'est pas un GUID valide");
            }

            return userId;
        }

        /// <summary>
        /// Extrait le nom d'utilisateur depuis les claims du token JWT (optionnel)
        /// </summary>
        /// <param name="user">ClaimsPrincipal contenant les claims du token</param>
        /// <returns>Nom d'utilisateur ou null</returns>
        public static string? GetUserNameFromClaims(ClaimsPrincipal user)
        {
            var nameClaim = user.FindFirst(ClaimTypes.Name)
                         ?? user.FindFirst("name");

            return nameClaim?.Value;
        }
    }
}
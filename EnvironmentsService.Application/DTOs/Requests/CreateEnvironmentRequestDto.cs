namespace EnvironmentsService.Application.DTOs.Requests
{
    public class CreateEnvironmentRequestDto
    {
        // Ce que l'utilisateur doit fournir pour créer un environnement
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
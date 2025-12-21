namespace EnvironmentsService.Application.DTOs.Requests
{
    public class UpdateEnvironmentRequestDto
    {
        // Ce que l'utilisateur peut modifier
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
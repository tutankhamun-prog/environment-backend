namespace EnvironmentsService.Domain.Entities
{
    public class Environment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public Guid CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation property vers les membres
        public ICollection<EnvironmentMember> Members { get; set; } = new List<EnvironmentMember>();
    }
}
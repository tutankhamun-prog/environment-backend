using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnvironmentsService.Domain.Entities;

namespace EnvironmentsService.Infrastructure.Data.Configurations
{
    public class EnvironmentConfiguration : IEntityTypeConfiguration<Domain.Entities.Environment>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Environment> builder)
        {
            builder.ToTable("Environments");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Description)
                .HasMaxLength(1000);

            builder.Property(e => e.CreatedBy)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Index pour améliorer les performances
            builder.HasIndex(e => e.CreatedBy);
            builder.HasIndex(e => e.CreatedAt);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnvironmentsService.Domain.Entities;

namespace EnvironmentsService.Infrastructure.Data.Configurations
{
    public class EnvironmentMemberConfiguration : IEntityTypeConfiguration<EnvironmentMember>
    {
        public void Configure(EntityTypeBuilder<EnvironmentMember> builder)
        {
            builder.ToTable("EnvironmentMembers");

            builder.HasKey(em => em.Id);

            // Relations
            builder.HasOne(em => em.Environment)
                .WithMany(e => e.Members)
                .HasForeignKey(em => em.EnvironmentId)
                .OnDelete(DeleteBehavior.Cascade); // Si l'environnement est supprimé, supprimer les membres

            // Propriétés
            builder.Property(em => em.UserId)
                .IsRequired();

            builder.Property(em => em.Role)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Member");

            builder.Property(em => em.AddedAt)
                .IsRequired();

            builder.Property(em => em.AddedBy)
                .IsRequired();

            // Index pour améliorer les performances
            builder.HasIndex(em => em.EnvironmentId);
            builder.HasIndex(em => em.UserId);

            // Index unique : un user ne peut être ajouté qu'une seule fois dans un environnement
            builder.HasIndex(em => new { em.EnvironmentId, em.UserId })
                .IsUnique();
        }
    }
}
using Microsoft.EntityFrameworkCore;
using EnvironmentsService.Domain.Entities;
using Environment = EnvironmentsService.Domain.Entities.Environment;

namespace EnvironmentsService.Infrastructure.Data.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Environment> Environments { get; set; }
        public DbSet<EnvironmentMember> EnvironmentMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
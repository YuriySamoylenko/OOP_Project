using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pms.Core.Entities;

namespace Pms.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User>(options)
    {
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<Sprint> Sprints => Set<Sprint>();
        public DbSet<PmsTask> Tasks => Set<PmsTask>();
        public DbSet<Participant> Participants => Set<Participant>();

        public DbSet<SystemAdmin> SystemAdmins => Set<SystemAdmin>();

        public DbSet<Manager> Managers => Set<Manager>();
        public DbSet<Member> Memberss => Set<Member>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<User>("User")
                .HasValue<SystemAdmin>("SystemAdmin");

            modelBuilder.Entity<Participant>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<Manager>("Manager")
                .HasValue<Member>("Member");

            modelBuilder.Entity<User>(e => e.Property(p => p.FirstName).IsRequired().HasMaxLength(256));
            modelBuilder.Entity<User>(e => e.Property(p => p.LastName).IsRequired().HasMaxLength(256));

            modelBuilder.Entity<Project>(e => e.Property(p => p.Name).IsRequired().HasMaxLength(256));
            modelBuilder.Entity<Project>(e => e.Property(p => p.Code).IsRequired().HasMaxLength(16));

            modelBuilder.Entity<Sprint>(e => e.Property(p => p.Name).IsRequired().HasMaxLength(16));

            modelBuilder.Entity<PmsTask>(e => e.Property(p => p.Summary).IsRequired().HasMaxLength(256));
            modelBuilder.Entity<PmsTask>(e => e.Property(p => p.Description).IsRequired().HasMaxLength(4000));
        }
    }
}

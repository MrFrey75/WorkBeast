using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkBeast.Core.Models;

namespace WorkBeast.Data.Context
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Exercise> Exercises => Set<Exercise>();
        public DbSet<BodyPart> BodyParts => Set<BodyPart>();
        public DbSet<WorkoutSession> WorkoutSessions => Set<WorkoutSession>();
        public DbSet<LoggedExercise> LoggedExercises => Set<LoggedExercise>();
        public DbSet<WorkoutSet> WorkoutSets => Set<WorkoutSet>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure primary keys for all entities
            modelBuilder.Entity<Exercise>().HasKey(e => e.Oid);
            modelBuilder.Entity<BodyPart>().HasKey(bp => bp.Oid);
            modelBuilder.Entity<WorkoutSession>().HasKey(ws => ws.Oid);
            modelBuilder.Entity<LoggedExercise>().HasKey(le => le.Oid);
            modelBuilder.Entity<WorkoutSet>().HasKey(ws => ws.Oid);

            // Configure Identity tables (optional customization)
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(u => u.LastName).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<ApplicationRole>(entity =>
            {
                entity.Property(r => r.Description).HasMaxLength(500);
            });

            // Exercise - BodyPart many-to-many relationship
            modelBuilder.Entity<ExerciseBodyPart>()
                .HasKey(eb => new { eb.ExerciseOid, eb.BodyPartOid });

            modelBuilder.Entity<ExerciseBodyPart>()
                .HasOne(eb => eb.Exercise)
                .WithMany()
                .HasForeignKey(eb => eb.ExerciseOid)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExerciseBodyPart>()
                .HasOne(eb => eb.BodyPart)
                .WithMany()
                .HasForeignKey(eb => eb.BodyPartOid)
                .OnDelete(DeleteBehavior.Cascade);

            // WorkoutSession -> LoggedExercise relationship
            modelBuilder.Entity<LoggedExercise>()
                .HasOne<WorkoutSession>()
                .WithMany(ws => ws.LoggedExercises)
                .HasForeignKey(le => le.WorkoutSessionOid)
                .OnDelete(DeleteBehavior.Cascade);

            // LoggedExercise -> Exercise relationship
            modelBuilder.Entity<LoggedExercise>()
                .HasOne(le => le.Exercise)
                .WithMany()
                .HasForeignKey(le => le.ExerciseOid)
                .OnDelete(DeleteBehavior.Restrict);

            // LoggedExercise -> WorkoutSet relationship
            modelBuilder.Entity<WorkoutSet>()
                .HasOne<LoggedExercise>()
                .WithMany(le => le.Sets)
                .HasForeignKey(ws => ws.LoggedExerciseOid)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure string lengths and constraints
            modelBuilder.Entity<Exercise>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(1000);
            });

            modelBuilder.Entity<BodyPart>(entity =>
            {
                entity.Property(bp => bp.Name).HasMaxLength(100).IsRequired();
                entity.Property(bp => bp.Description).HasMaxLength(500);
            });

            modelBuilder.Entity<WorkoutSession>(entity =>
            {
                entity.Property(ws => ws.Notes).HasMaxLength(2000);
            });

            modelBuilder.Entity<WorkoutSet>(entity =>
            {
                entity.Property(ws => ws.SetType).HasMaxLength(50).IsRequired();
            });

            // Indexes for performance
            modelBuilder.Entity<Exercise>()
                .HasIndex(e => e.Name);

            modelBuilder.Entity<Exercise>()
                .HasIndex(e => e.IsDeleted);

            modelBuilder.Entity<BodyPart>()
                .HasIndex(bp => bp.Name);

            modelBuilder.Entity<WorkoutSession>()
                .HasIndex(ws => ws.Date);

            modelBuilder.Entity<WorkoutSession>()
                .HasIndex(ws => ws.IsDeleted);

            modelBuilder.Entity<LoggedExercise>()
                .HasIndex(le => le.Sequence);

            modelBuilder.Entity<LoggedExercise>()
                .HasIndex(le => new { le.WorkoutSessionOid, le.Sequence });

            modelBuilder.Entity<WorkoutSet>()
                .HasIndex(ws => new { ws.LoggedExerciseOid, ws.SortOrder });

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.IsActive);

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.CreatedAt);
        }
    }
}

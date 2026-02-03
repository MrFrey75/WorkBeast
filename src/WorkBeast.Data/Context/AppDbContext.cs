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

            // Exercise - BodyPart many-to-many relationship
            modelBuilder.Entity<ExerciseBodyPart>()
                .HasKey(eb => new { eb.ExerciseOid, eb.BodyPartOid });

            modelBuilder.Entity<ExerciseBodyPart>()
                .HasOne(eb => eb.Exercise)
                .WithMany()
                .HasForeignKey(eb => eb.ExerciseOid);

            modelBuilder.Entity<ExerciseBodyPart>()
                .HasOne(eb => eb.BodyPart)
                .WithMany()
                .HasForeignKey(eb => eb.BodyPartOid);

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

            // Indexes for performance
            modelBuilder.Entity<Exercise>()
                .HasIndex(e => e.Name);

            modelBuilder.Entity<WorkoutSession>()
                .HasIndex(ws => ws.Date);

            modelBuilder.Entity<LoggedExercise>()
                .HasIndex(le => le.Sequence);
        }
    }
}

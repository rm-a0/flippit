using Flippit.Api.DAL.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flippit.Api.DAL.EF
{
    public class FlippitDbContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; } = null!;
        public DbSet<CardEntity> Cards { get; set; } = null!;
        public DbSet<CollectionEntity> Collections { get; set; } = null!;
        public DbSet<CompletedLessonEntity> CompletedLessons { get; set; } = null!;

        public FlippitDbContext(DbContextOptions<FlippitDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CardEntity>()
                .HasOne<UserEntity>()
                .WithMany()
                .HasForeignKey(nameof(CardEntity.CreatorId))
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CardEntity>()
                .HasOne<CollectionEntity>()
                .WithMany()
                .HasForeignKey(nameof(CardEntity.CollectionId))
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CollectionEntity>()
                .HasOne<UserEntity>()
                .WithMany()
                .HasForeignKey(nameof(CollectionEntity.CreatorId))
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CompletedLessonEntity>()
                .HasOne<CollectionEntity>()
                .WithMany()
                .HasForeignKey(nameof(CompletedLessonEntity.CollectionId))
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CompletedLessonEntity>()
                .HasOne<UserEntity>()
                .WithMany()
                .HasForeignKey(nameof(CompletedLessonEntity.UserId))
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserEntity>().HasIndex(nameof(UserEntity.Name));
            modelBuilder.Entity<CollectionEntity>().HasIndex(nameof(CollectionEntity.Name));
        }
    }
}

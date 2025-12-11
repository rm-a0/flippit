using Flippit.Api.DAL.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flippit.Api.DAL.EF
{
    public class FlippitDbContext : DbContext
    {
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
                .HasOne<CollectionEntity>()
                .WithMany()
                .HasForeignKey(nameof(CardEntity.CollectionId))
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CompletedLessonEntity>()
                .HasOne<CollectionEntity>()
                .WithMany()
                .HasForeignKey(nameof(CompletedLessonEntity.CollectionId))
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CollectionEntity>().HasIndex(nameof(CollectionEntity.Name));
        }
    }
}

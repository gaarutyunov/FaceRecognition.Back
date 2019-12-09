using FaceRecognition.Back.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FaceRecognition.Back.Api.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User>? Users { get; set; }
        public DbSet<File>? Files { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<File>()
                .HasOne(x => x.User!)
                .WithMany(x => x.Files!)
                .HasForeignKey(x => x.UserId);
        }
    }
}
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
    }
}
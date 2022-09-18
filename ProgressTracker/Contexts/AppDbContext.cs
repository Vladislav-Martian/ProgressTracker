using Microsoft.EntityFrameworkCore;
using ProgressTracker.Models;

namespace ProgressTracker.Contexts
{
    public class AppDbContext : DbContext
    {
        #region Tables
        public DbSet<TaskModel> TaskModels { get; set; }
        public DbSet<UserReference> UserReferences { get; set; }
        #endregion

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

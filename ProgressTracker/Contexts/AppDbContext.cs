using Microsoft.EntityFrameworkCore;
using ProgressTracker.Models;

namespace ProgressTracker.Contexts
{
    public class AppDbContext : DbContext
    {
        #region Tables
        public DbSet<UserModel> UserModels { get; set; }
        #endregion

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}

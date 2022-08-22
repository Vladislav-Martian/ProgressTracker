﻿using Microsoft.EntityFrameworkCore;

namespace ProgressTracker.Contexts
{
    public class AppDbContext : DbContext
    {
        #region Tables

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
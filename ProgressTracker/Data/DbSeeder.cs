using ProgressTracker.Contexts;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProgressTracker.Data
{
    public class DbSeeder
    {
        public static void Seed(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<AppDbContext>();
                context?.Database.EnsureCreated();
                if (context != null) 
                {
                    DbSeed(context);
                    context.SaveChanges();
                }
            }
        }

        public static void DbSeed(DbContext db)
        {
            // just add the content here
        }
    }
}

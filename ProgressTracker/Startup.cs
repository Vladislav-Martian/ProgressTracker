using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProgressTracker.Contexts;
using System;
using ProgressTracker.Data;

namespace ProgressTracker
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Controllers + newtonsoft json serializer
            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            services.AddEndpointsApiExplorer();
            // Swagger configuration:
            var version = Configuration["Meta:Version"];
            var appname = Configuration["Meta:Name"]; // additionaly used later as a name for db
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(version, new OpenApiInfo { Title = appname, Version = version });
            });
            // Main context of db, in-memory based
            services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseInMemoryDatabase(appname);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            // Without dev-mode condition because it is just demo-project
            app.UseSwagger();
            // Example endpoint: «/swagger/v1.0.0/swagger.json»
            app.UseSwaggerUI(c => c.SwaggerEndpoint(GetSwaggerEndpoint(), string.Concat(
                Configuration["Meta:Name"].ToString(),
                " ",
                Configuration["Meta:Version"].ToString())));

            app.UseRouting();
            app.UseHttpsRedirection();
            
            // Authorization and others
            app.UseAuthorization();

            // Mapping
            app.UseEndpoints(ConfigureRoutes);

            // Database seeding
            DbSeeder.Seed(app);
        }

        #region Helper methods
        private void ConfigureRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllers();
        }

        private string GetSwaggerEndpoint()
        {
            string versionWithoutPoints = Configuration["Meta:Version"].ToString();
            return $"/swagger/{versionWithoutPoints}/swagger.json";
        }
        #endregion
    }
}

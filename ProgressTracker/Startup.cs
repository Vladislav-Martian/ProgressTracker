using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProgressTracker.Contexts;
using System;
using ProgressTracker.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

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
            services.AddDbContext<AuthDbContext>(opt =>
            {
                opt.UseInMemoryDatabase(appname);
            });
            // Authentication
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                var Key = Encoding.UTF8.GetBytes(Configuration["JWT:Key"]);
                o.SaveToken = true;
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["JWT:Issuer"],
                    ValidAudience = Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Key)
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Without dev-mode condition because it is just demo-project
            app.UseSwagger();
            // Example endpoint: «/swagger/v1.0.0/swagger.json»
            app.UseSwaggerUI(c => c.SwaggerEndpoint(GetSwaggerEndpoint(), string.Concat(
                Configuration["Meta:Name"].ToString(),
                " ",
                Configuration["Meta:Version"].ToString())));

            app.UseHttpsRedirection();
            app.UseRouting();

            // Authorization and others
            app.UseAuthentication();
            app.UseAuthorization();

            // Mapping
            app.UseEndpoints(ConfigureRoutes);

            // Database seeding
            DbSeeder.Seed(app);
            Task.Run(async () => await RoleSeeder.SeedAsync(userManager, roleManager));
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

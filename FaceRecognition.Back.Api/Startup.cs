using AutoMapper;
using FaceRecognition.Back.Api.Contexts;
using FaceRecognition.Back.Api.Extensions;
using FaceRecognition.Back.Api.Hubs;
using FaceRecognition.Back.Api.Interfaces;
using FaceRecognition.Back.Api.MiddleWares;
using FaceRecognition.Back.Api.Profiles;
using FaceRecognition.Back.Api.Services;
using FaceRecognition.Back.Api.StartupJobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace FaceRecognition.Back.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddAppCors(Configuration);

            services.AddJwt(Configuration);
            services.AddSignalR().AddHubOptions<QrHub>(x => x.EnableDetailedErrors = true);

            services.AddAutoMapper(typeof(DomainToDtoProfile));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IQrService, QrService>();
            services.AddTransient<IStartupJob, EnsureDbData>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "FaceRecognition", Version = "v1"});
            });

            services.AddEntityFrameworkNpgsql()
                .AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseRouting();

            app.UseDefaultCors(Configuration);

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "FaceRecognition V1"); });

            app.UseAuthorization();
            app.UseAuthentication();
            app.UseMiddleware<AppExceptionMiddleWare>();

            var jobs = app.ApplicationServices.GetServices<IStartupJob>();
            foreach (var startupJob in jobs)
            {
                using var scope = app.ApplicationServices.CreateScope();
                startupJob.Invoke(scope.ServiceProvider);
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<QrHub>("/qrHub");
            });
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FaceRecognition.Back.Api.Contexts;
using FaceRecognition.Back.Api.Extensions;
using FaceRecognition.Back.Api.Interfaces;
using FaceRecognition.Back.Api.MiddleWares;
using FaceRecognition.Back.Api.Models;
using FaceRecognition.Back.Api.Profiles;
using FaceRecognition.Back.Api.Services;
using FaceRecognition.Back.Api.StartupJobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

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
            services.AddCors();

            services.AddJwt(Configuration);

            services.AddAutoMapper(typeof(DomainToDtoProfile));
            services.AddScoped<IUserService, UserService>();
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

            app.UseCors(x =>
            {
                x.AllowAnyHeader();
                x.AllowAnyOrigin();
                x.AllowAnyMethod();
            });

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

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
using System.Text;
using FaceRecognition.Back.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace FaceRecognition.Back.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.UTF8.GetBytes(appSettings.Secret);

            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            return services;
        }
        
        public static IServiceCollection AddAppCors(this IServiceCollection services, IConfiguration configuration)
        {
            var corsSettingsSection = configuration.GetSection("CorsSettings");
            services.Configure<CorsSettings>(corsSettingsSection);
            var corsSettings = corsSettingsSection.Get<CorsSettings>();
            services.AddCors(options =>
            {
                foreach (var (key, value) in corsSettings.Policies)
                {
                    options.AddPolicy(key, builder =>
                    {
                        builder.WithHeaders(value.Headers)
                            .WithMethods(value.Methods)
                            .WithOrigins(value.Origins);

                        if (value.AllowCredentials) builder.AllowCredentials();
                    });
                }
            });
            return services;
        }
    }
}
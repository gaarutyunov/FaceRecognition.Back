using FaceRecognition.Back.Api.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace FaceRecognition.Back.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDefaultCors(this IApplicationBuilder app, IConfiguration configuration)
        {
            var corsSettingsSection = configuration.GetSection("CorsSettings");
            var corsSettings = corsSettingsSection.Get<CorsSettings>();
            var defaultCors = corsSettings.DefaultCors;

            if (!string.IsNullOrEmpty(defaultCors) && corsSettings.Policies.ContainsKey(defaultCors))
            {
                app.UseCors(defaultCors);
            }
            
            return app;
        }
    }
}
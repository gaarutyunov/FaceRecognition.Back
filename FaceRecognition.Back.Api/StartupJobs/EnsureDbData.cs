using System;
using System.Linq;
using FaceRecognition.Back.Api.Contexts;
using FaceRecognition.Back.Api.Dtos;
using FaceRecognition.Back.Api.Interfaces;
using FaceRecognition.Back.Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace FaceRecognition.Back.Api.StartupJobs
{
    public class EnsureDbData : IStartupJob
    {

        public EnsureDbData()
        {
        }

        public void Invoke(IServiceProvider serviceProvider)
        {
            using var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (context.Users.Any(x => x.Login == "admin")) return;
            
            var service = serviceProvider.GetRequiredService<IUserService>();
            service.Register(new CreateUserDto
            {
                Login = "admin",
                Password = "admin"
            }).Wait();
        }
    }
}
using System;

namespace FaceRecognition.Back.Api.Interfaces
{
    public interface IStartupJob
    {
        void Invoke(IServiceProvider serviceProvider);
    }
}
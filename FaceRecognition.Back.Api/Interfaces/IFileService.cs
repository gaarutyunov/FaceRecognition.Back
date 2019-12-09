using System;
using System.Threading.Tasks;
using FaceRecognition.Back.Api.Models;

namespace FaceRecognition.Back.Api.Interfaces
{
    public interface IFileService
    {
        Task<File> WriteFileAsync(Guid userId, string fileContent);
        Task<string> ReadFileAsync(File file);
        Task SaveFileAsync(File file);
    }
}
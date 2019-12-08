using System;
using System.Threading.Tasks;

namespace FaceRecognition.Back.Api.Interfaces
{
    public interface IFileService
    {
        Task<Guid> WriteFileAsync(Guid userId, string fileContent);
        Task SaveFileAsync(Guid userId, Guid fileId);
    }
}
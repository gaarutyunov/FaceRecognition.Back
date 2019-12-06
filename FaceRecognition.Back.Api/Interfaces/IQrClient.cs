using System.Threading.Tasks;

namespace FaceRecognition.Back.Api.Interfaces
{
    public interface IQrClient
    {
        Task GenerateQrUrl(string url);
    }
}
using System.Threading.Tasks;

namespace FaceRecognition.Back.Api.Interfaces
{
    public interface IFaceRecognitionService
    {
        bool CompareFaces(byte[] savedImage, byte[] imageToCheck);
    }
}
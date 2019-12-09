using System.Threading.Tasks;

namespace FaceRecognition.Back.Api.Interfaces
{
    public interface IFaceRecognitionService
    {
        bool CompareFaces(string savedImage, string imageToCheck);
        void Initialize();
    }
}
namespace FaceRecognition.Back.Api.Interfaces
{
    public interface IQrService
    {
        string GenerateQrUrl(string url, int pixelsPerModule = 20);
    }
}
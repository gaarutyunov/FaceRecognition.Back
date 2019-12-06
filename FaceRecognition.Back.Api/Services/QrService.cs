using FaceRecognition.Back.Api.Interfaces;
using QRCoder;

namespace FaceRecognition.Back.Api.Services
{
    public class QrService : IQrService
    {
        public string GenerateQrUrl(string url, int pixelsPerModule = 20)
        {
            var generator = new PayloadGenerator.Url(url);
            var payload = generator.ToString();
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new Base64QRCode(qrCodeData);
            return qrCode.GetGraphic(pixelsPerModule);
        }
    }
}
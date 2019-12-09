using FaceRecognition.Back.Api.Enums;

namespace FaceRecognition.Back.Api.Models
{
    public class FileData
    {
        public byte[] Content { get; set; }
        public MimeType MimeType { get; set; }

        public FileData()
        {
            Content = new byte[0];
        }
    }
}
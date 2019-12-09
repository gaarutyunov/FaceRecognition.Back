using System;
using FaceRecognition.Back.Api.Enums;

namespace FaceRecognition.Back.Api.Models
{
    public class File
    {
        public Guid Id { get; set; }
        
        public Guid UserId { get; set; }
        public User? User { get; set; }
        
        public int Size { get; set; }
        public MimeType MimeType { get; set; }
    }
}
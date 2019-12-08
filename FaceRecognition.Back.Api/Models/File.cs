using System;

namespace FaceRecognition.Back.Api.Models
{
    public class File
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
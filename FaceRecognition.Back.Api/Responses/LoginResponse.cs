using System;

namespace FaceRecognition.Back.Api.Responses
{
    public class LoginResponse
    {
        public Guid Id { get; set; }
        public string? Login { get; set; }
        public string? FilePath { get; set; }
        public string? FileToCheckPath { get; set; }
    }
}
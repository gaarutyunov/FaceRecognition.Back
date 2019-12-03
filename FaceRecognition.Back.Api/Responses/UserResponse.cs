using System;

namespace FaceRecognition.Back.Api.Responses
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string? Login { get; set; }
    }
}
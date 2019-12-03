using System.ComponentModel.DataAnnotations;

namespace FaceRecognition.Back.Api.Dtos
{
    public class CreateUserDto
    {
        [Required]
        public string? Login { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
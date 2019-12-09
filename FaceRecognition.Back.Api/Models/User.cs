using System;
using System.Collections.Generic;

namespace FaceRecognition.Back.Api.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Login { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public ICollection<File>? Files { get; set; }

        public User()
        {
            PasswordHash = new byte[0];
            PasswordSalt = new byte[0];
        }
    }
}
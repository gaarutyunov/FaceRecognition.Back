using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaceRecognition.Back.Api.Contexts;
using FaceRecognition.Back.Api.Dtos;
using FaceRecognition.Back.Api.Enums;
using FaceRecognition.Back.Api.Exceptions;
using FaceRecognition.Back.Api.Extensions;
using FaceRecognition.Back.Api.Interfaces;
using FaceRecognition.Back.Api.Models;
using FaceRecognition.Back.Api.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FaceRecognition.Back.Api.Services
{
    internal class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;
        private readonly IFileService _fileService;

        public UserService(
            ApplicationDbContext context,
            ILogger<IUserService> logger,
            IFileService fileService)
        {
            _context = context;
            _logger = logger;
            _fileService = fileService;
        }

        public async Task<UserResponse> Register(CreateUserDto dto)
        {
            if (dto.Login == null) throw new ArgumentNullException(nameof(dto.Login));
            if (dto.Password == null) throw new ArgumentNullException(nameof(dto.Password));
            if (dto.File == null) throw new ArgumentNullException(nameof(dto.File));
            
            await using var context = _context;
            var userExists = await context.Users.AnyAsync(x => x.Login == dto.Login);

            if (userExists) throw new AlreadyExistsException(EntityType.USER);

            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Login = dto.Login
            };
            
            CreatePasswordHash(dto.Password, out var passwordHash, out var passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
                
            await context.Users!.AddAsync(user);
            await context.SaveChangesAsync();

            var file = await _fileService.WriteFileAsync(userId, dto.File);
            await _fileService.SaveFileAsync(file);
            
            _logger.LogInformation($"User registered {user.Login}");

            return new UserResponse
            {
                Id = user.Id,
                Login = user.Login
            };
        }

        public async Task<LoginResponse> Login(LoginUserDto dto)
        {
            if (dto.Login == null) throw new ArgumentNullException(nameof(dto.Login));
            if (dto.Password == null) throw new ArgumentNullException(nameof(dto.Password));
            if (dto.File == null) throw new ArgumentNullException(nameof(dto.File));
            
            await using var context = _context;
            var user = await context.Users.FirstOrDefaultAsync(x => x.Login == dto.Login);

            if (user == null) throw new NotFoundException(EntityType.USER);
            if (!VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt)) throw new AuthorizationException(ExceptionSubType.PASSWORD_INCORRECT);

            var file = await context.Files.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (file == null) throw new NotFoundException(EntityType.FILE);
            var res = _fileService.WriteFileAsync(user.Id, dto.File).Result;

            var fileToCheck = res;

            return new LoginResponse
            {
                Id = user.Id,
                Login = user.Login,
                FilePath = Path.Combine("Images", file.GetPath),
                FileToCheckPath = Path.Combine("Images", fileToCheck.GetPath)
            };
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));
            if (storedHash.Length != 64) throw new ArgumentException("Illegal hash length", nameof(storedHash));
            if (storedSalt.Length != 128) throw new ArgumentException("Illegal salt length", nameof(storedSalt));
            
            using var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return !computedHash.Where((t, i) => t != storedHash[i]).Any();
        }
    }
}
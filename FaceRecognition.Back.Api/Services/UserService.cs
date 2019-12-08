using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaceRecognition.Back.Api.Contexts;
using FaceRecognition.Back.Api.Dtos;
using FaceRecognition.Back.Api.Enums;
using FaceRecognition.Back.Api.Exceptions;
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

        public async Task<UserResponse> Register(CreateUserDto createUserDto)
        {
            await using var context = _context;
            var userExists = await context.Users.AnyAsync(x => x.Login == createUserDto.Login);

            if (userExists) throw new AlreadyExistsException(EntityType.USER);

            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Login = createUserDto.Login
            };
            
            CreatePasswordHash(createUserDto.Password, out var passwordHash, out var passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
                
            await context.Users!.AddAsync(user);
            await context.SaveChangesAsync();

            var fileId = await _fileService.WriteFileAsync(userId, createUserDto.File);
            await _fileService.SaveFileAsync(userId, fileId);
            
            _logger.LogInformation($"User registered {user.Login}");

            return new UserResponse
            {
                Id = user.Id,
                Login = user.Login
            };
        }

        public async Task<UserResponse> Login(LoginUserDto dto)
        {
            await using var context = _context;
            var user = await context.Users.FirstOrDefaultAsync(x => x.Login == dto.Login);

            if (user == null) throw new NotFoundException(EntityType.USER);
            if (!VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt)) throw new AuthorizationException(ExceptionSubType.PASSWORD_INCORRECT);

            return new UserResponse
            {
                Id = user.Id,
                Login = user.Login
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
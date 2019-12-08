using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FaceRecognition.Back.Api.Contexts;
using FaceRecognition.Back.Api.Enums;
using FaceRecognition.Back.Api.Exceptions;
using FaceRecognition.Back.Api.Interfaces;
using File = FaceRecognition.Back.Api.Models.File;

namespace FaceRecognition.Back.Api.Services
{
    public class FileService : IFileService
    {
        private const string ImageDirectory = "Images";
        private const string ImageExt = "png";
        private readonly ApplicationDbContext _dbContext;

        public FileService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> WriteFileAsync(Guid userId, string fileContent)
        {
            var fileId = Guid.NewGuid();
            var userFolder = Path.Combine(ImageDirectory, userId.ToString());
            if (!Directory.Exists(userFolder))
            {
                Directory.CreateDirectory(userFolder);
            }

            var filePath = Path.Combine(
                ImageDirectory,
                userId.ToString(),
                $"{fileId}.{ImageExt}");
            var base64Data = Regex.Match(
                    fileContent,
                    @"data:image/(?<type>.+?),(?<data>.+)")
                .Groups["data"]
                .Value;
            var binData = Convert.FromBase64String(base64Data);

            await using var sourceStream = new FileStream(
                filePath,
                FileMode.OpenOrCreate,
                FileAccess.Write,
                FileShare.None,
                bufferSize: binData.Length,
                useAsync: true);
            await sourceStream.WriteAsync(binData, 0, binData.Length);

            return fileId;
        }

        public async Task SaveFileAsync(Guid userId, Guid fileId)
        {
            await using var context = _dbContext;

            if (!context.Users.Any(x => x.Id == userId)) throw new NotFoundException(EntityType.USER);
            var file = new File
            {
                Id = fileId,
                UserId = userId
            };

            await context.Files!.AddAsync(file);
            await context.SaveChangesAsync();
        }
    }
}
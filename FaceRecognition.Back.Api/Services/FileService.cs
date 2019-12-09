using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FaceRecognition.Back.Api.Contexts;
using FaceRecognition.Back.Api.Dictionaries;
using FaceRecognition.Back.Api.Enums;
using FaceRecognition.Back.Api.Exceptions;
using FaceRecognition.Back.Api.Extensions;
using FaceRecognition.Back.Api.Interfaces;
using File = FaceRecognition.Back.Api.Models.File;

namespace FaceRecognition.Back.Api.Services
{
    public class FileService : IFileService
    {
        private const string IMAGE_DIRECTORY = "Images";
        private readonly ApplicationDbContext _dbContext;

        public FileService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<File> WriteFileAsync(Guid userId, string fileContent)
        {
            var fileId = Guid.NewGuid();
            var userFolder = Path.Combine(IMAGE_DIRECTORY, userId.ToString());
            if (!Directory.Exists(userFolder))
            {
                Directory.CreateDirectory(userFolder);
            }

            var fileData = fileContent.FileDataFromDataUrl();
            var binData = fileData.Content;

            var filePath = Path.Combine(
                IMAGE_DIRECTORY,
                userId.ToString(),
                $"{fileId}.{MimeTypeDictionary.GetType(fileData.MimeType)}");

            await using var sourceStream = new FileStream(
                filePath,
                FileMode.OpenOrCreate,
                FileAccess.Write,
                FileShare.None,
                bufferSize: binData.Length,
                useAsync: true);
            
            await sourceStream.WriteAsync(binData, 0, binData.Length);

            return new File
            {
                Id = fileId,
                MimeType = fileData.MimeType,
                Size = binData.Length,
                UserId = userId
            };
        }

        public async Task<string> ReadFileAsync(Guid userId, File file)
        {
            var userFolder = Path.Combine(IMAGE_DIRECTORY, userId.ToString());
            if (!Directory.Exists(userFolder)) throw new NotFoundException(EntityType.FOLDER);

            var filePath = Path.Combine(
                IMAGE_DIRECTORY,
                userId.ToString(),
                $"{file.Id}.{MimeTypeDictionary.GetType(file.MimeType)}");
            
            await using var sourceStream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.None,
                bufferSize: file.Size,
                useAsync: true);

            var sb = new StringBuilder();

            var buffer = new byte[file.Size];
            int numRead;
            while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                var text = Encoding.Unicode.GetString(buffer, 0, numRead);
                sb.Append(text);
            }

            return sb.ToString();
        }

        public async Task SaveFileAsync(File file)
        {
            await using var context = _dbContext;

            if (!context.Users.Any(x => x.Id == file.UserId)) throw new NotFoundException(EntityType.USER);

            await context.Files!.AddAsync(file);
            await context.SaveChangesAsync();
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using FaceRecognition.Back.Api.Interfaces;
using Microsoft.Extensions.Logging;

namespace FaceRecognition.Back.Api.Services
{
    public class FaceRecognitionService : IFaceRecognitionService
    {
        private FaceRecognitionDotNet.FaceRecognition? _faceRecognition;
        private readonly ILogger _logger;
        private const string MODEL_DIRECTORY = "FRModels";
        private const string MODEL_BASE_URL = "https://github.com/ageitgey/face_recognition_models/raw/master/face_recognition_models/models";
        private IList<string> ModelFiles = new List<string>();
        public FaceRecognitionService(ILogger<FaceRecognitionService> logger)
        {
            _logger = logger;
        }

        public void Initialize()
        {
            var faceRecognition = typeof(FaceRecognitionDotNet.FaceRecognition);
            var type = faceRecognition.Assembly.GetTypes().FirstOrDefault(t => t.Name == "FaceRecognitionModels");
            if (type == null) throw new ArgumentNullException();

            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);

            var models = new List<string>();
            
            foreach (var method in methods)
            {
//                 Skip helen
                if (method.Name == "GetPosePredictor194PointModelLocation")
                    continue;

                var result = method!.Invoke(null, BindingFlags.Public | BindingFlags.Static, null, null, null) as string;
                if (string.IsNullOrWhiteSpace(result))
                {
                    _logger.LogError($"{method.Name} does not return {typeof(string).FullName} value or return null or whitespace value.");
                    throw new ArgumentNullException(nameof(result));
                }

                models.Add(result);

                var path = Path.Combine(MODEL_DIRECTORY, result);
                if (File.Exists(path))
                    continue;

                var binary = new HttpClient().GetByteArrayAsync($"{MODEL_BASE_URL}/{result}").Result;

                Directory.CreateDirectory(MODEL_DIRECTORY);
                File.WriteAllBytes(path, binary);
                
                foreach (var model in models)
                    ModelFiles.Add(model);
            }
            
            _faceRecognition = FaceRecognitionDotNet.FaceRecognition.Create(MODEL_DIRECTORY);
        }

        public bool CompareFaces(string savedImage, string imageToCheck)
        {
            using var image1 = FaceRecognitionDotNet.FaceRecognition.LoadImageFile(savedImage);
            using var image2 = FaceRecognitionDotNet.FaceRecognition.LoadImageFile(imageToCheck);

            var encodings1 = _faceRecognition!.FaceEncodings(image1).ToArray();
            var encodings2 = _faceRecognition!.FaceEncodings(image2).ToArray();

            var areEqual = false;
            
            foreach (var faceEncoding in encodings1)
            {
                foreach (var compareFace in FaceRecognitionDotNet.FaceRecognition.CompareFaces(encodings2, faceEncoding))
                {
                    areEqual = compareFace;
                    if (!areEqual) break;
                }
            }
            
            foreach (var encoding in encodings1)
                encoding.Dispose();
            foreach (var encoding in encodings2)
                encoding.Dispose();
            
            return areEqual;
        }
    }
}
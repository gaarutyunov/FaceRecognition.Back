using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FaceRecognition.Back.Api.Dictionaries;
using FaceRecognition.Back.Api.Models;

namespace FaceRecognition.Back.Api.Extensions
{
    public static class StringExtensions
    {
        private const string TYPE_LITERAL = "type";
        private const string DATA_LITERAL = "data";
        private const string DATA_PATTERN = @"data:image/(?<type>.+);base64,(?<data>.+)";

        public static byte[] AsBytes(this string str) => Encoding.UTF8.GetBytes(str);

        public static FileData FileDataFromDataUrl(this string str)
        {
            var groups = Regex.Match(
                str,
                DATA_PATTERN
            );
            var data = groups.Groups[DATA_LITERAL].Value;
            var type = groups.Groups[TYPE_LITERAL].Value;
            
            return new FileData
            {
                Content = Convert.FromBase64String(data),
                MimeType = MimeTypeDictionary.GetMimeType(type)
            };
        }
    }
}
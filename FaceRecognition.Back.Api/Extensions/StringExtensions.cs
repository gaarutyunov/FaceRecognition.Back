using System.Text;

namespace FaceRecognition.Back.Api.Extensions
{
    public static class StringExtensions
    {
        public static byte[] AsBytes(this string str) => Encoding.UTF8.GetBytes(str);
    }
}
using FaceRecognition.Back.Api.Enums;

namespace FaceRecognition.Back.Api.Exceptions
{
    public class AuthorizationException : AppException
    {
        public AuthorizationException(ExceptionSubType exceptionSubType) : base(EntityType.USER, ExceptionType.AUTHORIZATION, exceptionSubType)
        {
        }
    }
}
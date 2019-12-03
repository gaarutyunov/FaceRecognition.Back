using FaceRecognition.Back.Api.Enums;

namespace FaceRecognition.Back.Api.Exceptions
{
    internal class AlreadyExistsException : AppException
    {
        public AlreadyExistsException(EntityType entityType) : base(entityType, ExceptionType.ALREADY_EXISTS)
        {
        }
    }
}
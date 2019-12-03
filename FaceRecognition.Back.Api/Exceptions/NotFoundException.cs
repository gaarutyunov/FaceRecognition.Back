using FaceRecognition.Back.Api.Enums;

namespace FaceRecognition.Back.Api.Exceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException(EntityType entityType) : base(entityType, ExceptionType.NOT_FOUND)
        {
        }
    }
}
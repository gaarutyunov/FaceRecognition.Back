using System;
using FaceRecognition.Back.Api.Enums;
using Newtonsoft.Json;

namespace FaceRecognition.Back.Api.Exceptions
{
    public class AppException : Exception
    {
        public EntityType EntityType { get; set; }
        public ExceptionType ExceptionType { get; set; }
        public ExceptionSubType? ExceptionSubType { get; set; }
        public string? ExceptionData { get; set; }

        public AppException(EntityType entityType, ExceptionType exceptionType, ExceptionSubType exceptionSubType, string exceptionData)
        {
            EntityType = entityType;
            ExceptionType = exceptionType;
            ExceptionSubType = exceptionSubType;
            ExceptionData = exceptionData;
        }

        public AppException(EntityType entityType, ExceptionType exceptionType, ExceptionSubType exceptionSubType)
        {
            EntityType = entityType;
            ExceptionType = exceptionType;
            ExceptionSubType = exceptionSubType;
        }

        public AppException(EntityType entityType, ExceptionType exceptionType, string exceptionData)
        {
            EntityType = entityType;
            ExceptionType = exceptionType;
            ExceptionData = exceptionData;
        }

        public AppException(EntityType entityType, ExceptionType exceptionType)
        {
            EntityType = entityType;
            ExceptionType = exceptionType;
        }
    }
}
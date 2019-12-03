using System;
using System.Net;
using System.Threading.Tasks;
using FaceRecognition.Back.Api.Enums;
using FaceRecognition.Back.Api.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FaceRecognition.Back.Api.MiddleWares
{
    public class AppExceptionMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AppExceptionMiddleWare> _logger;

        public AppExceptionMiddleWare(RequestDelegate next, ILogger<AppExceptionMiddleWare> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(context, e);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var exc = MapException(exception);
            var serialized = JsonConvert.SerializeObject(exc.Data, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });
            context.Response.Clear();
            context.Response.StatusCode = (int) exc.StatusCode;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(serialized);
        }

        private static ExceptionObject MapException(Exception exception)
        {
            switch (exception)
            {
                case AppException appException:
                {
                    return new ExceptionObject
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Data = new AppExceptionData(appException.EntityType, appException.ExceptionType, appException.ExceptionData)
                    };
                }
                default:
                {
                    return new ExceptionObject
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Data = new AppExceptionData(EntityType.NONE, ExceptionType.DEFAULT)
                    };
                }
            }
        }

        private class ExceptionObject
        {
            public HttpStatusCode StatusCode { get; set; }
            public AppExceptionData? Data { get; set; }
        }
        
        private class AppExceptionData
        {
            public EntityType EntityType { get; set; }
            public ExceptionType ExceptionType { get; set; }
            public string? ExceptionData { get; set; }

            public AppExceptionData(EntityType entityType, ExceptionType exceptionType, string? exceptionData)
            {
                EntityType = entityType;
                ExceptionType = exceptionType;
                ExceptionData = exceptionData;
            }

            public AppExceptionData(EntityType entityType, ExceptionType exceptionType)
            {
                EntityType = entityType;
                ExceptionType = exceptionType;
            }
        }
    }
}
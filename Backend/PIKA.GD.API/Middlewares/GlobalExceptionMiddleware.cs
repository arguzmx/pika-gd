using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace PIKA.GD.API.Middlewares
{
    
    public class GlobalExceptionMiddleware
    {

        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"{ex.Message}");
                await HandleExceptionAsync(httpContext, ex);
            }


        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            APIErrorDetail detail = new APIErrorDetail();

            switch (exception.GetType())
            {
                case Type NFType when NFType == typeof(EXNoEncontrado):
                    detail.Message = exception.Message == null ? "": exception.Message; 
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                case Type InvDataType when InvDataType == typeof(ExElementoExistente):
                    detail.Message = exception.Message == null ? "" : exception.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    break;

                case Type InvDataType when InvDataType == typeof(ExErrorRelacional):
                    detail.Message = exception.Message == null ? "" : exception.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                case Type InvDataType when InvDataType == typeof(ExLoginInfoInvalida):
                    detail.Message = exception.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                //case Type ApiRefitException when ApiRefitException == typeof(ApiException):
                //    var ex = exception as ApiException;
                //    context.Response.StatusCode = (int)ex.StatusCode;
                //    detail.Message = ex.Message;
                //    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    detail.Message = exception.Message;
                    break;

            }

            detail.StatusCode = context.Response.StatusCode;



            return context.Response.WriteAsync(JsonSerializer.Serialize(detail));

        }


    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}

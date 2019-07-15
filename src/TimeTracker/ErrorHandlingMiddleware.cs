using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TimeTracker
{
    public class ErrorHandlingMiddleware
    {
        private RequestDelegate _next;
        private ILogger<ErrorHandlingMiddleware> _logger;
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            catch (Exception ex)
            {

                _logger.LogError(ex, ex.Message);

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected
            // TODO: Change code based on exception

            // Moze preko ovih if da se odradi
                //if (exception is NotFoundException) code = HttpStatusCode.NotFound;
                //else if (exception is UnauthorizedException) code = HttpStatusCode.Unauthorized;
                //else if (exception is CustomException) code = HttpStatusCode.BadRequest;

                // Using RFC 7807 response for error formatting
                // https://tools.ietf.org/html/rfc7807

            //Napravimo novu instancu problema RFC 47807
            //Type je jedinstveni identifikator greske, obicno neki link, pa kad se klikne na taj link izbace de dodatne informacije o gresci
            var problem = new ProblemDetails
            {
                Type = "https://yourdomain.com/errors/internal-server-error",
                Title = "Internal Server Error",
                Detail = exception.Message,
                Instance = "",
                Status = (int) code
            };

            // Da bi ovo vratili na klijenta, odradimo Json serialization, pa dobijemo JSON od svega ovog
            var result = JsonSerializer.ToString(problem);

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int) code;

            return context.Response.WriteAsync(result);
        }
    }
}

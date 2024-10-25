using DAM.Domain.Exceptions;
using DAM.WebApp.Models.Identity.OAuth.Exceptions;
using FluentValidation;
using Newtonsoft.Json;

namespace DAM.WebApp.Middlewares
{
    internal sealed class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) => _logger = logger;
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);

            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                await HandleExceptionAsync(context, e);
            }
        }
        private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            var statusCode = GetStatusCode(exception);
            var response = new
            {
                title = GetTitle(exception),
                status = statusCode,
                detail = exception.Message,
                errors = GetErrors(exception)
            };
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
        private static int GetStatusCode(Exception exception) =>
            exception switch
            {
                BadHttpRequestException => StatusCodes.Status400BadRequest,
                // NotFoundException => StatusCodes.Status404NotFound,
                ValidationException => StatusCodes.Status422UnprocessableEntity,
                InvalidDiscordUserException => StatusCodes.Status422UnprocessableEntity,
                InvalidEntityOperationException => StatusCodes.Status422UnprocessableEntity,
                _ => StatusCodes.Status500InternalServerError
            };
        private static string GetTitle(Exception exception) =>
            exception switch
            {
                ApplicationException applicationException => applicationException?.Source ?? "Server Error",
                _ => "Server Error"
            };
        private static IReadOnlyDictionary<string, string[]> GetErrors(Exception exception)
        {
            IReadOnlyDictionary<string, string[]> errors = null;
            if (exception is ValidationException validationException)
            {
                errors = validationException.Errors.GroupBy(
                                                x => x.PropertyName,
                                                x => x.ErrorMessage,
                                                (propertyName, errorMessages) => new
                                                {
                                                    Key = propertyName,
                                                    Values = errorMessages.Distinct().ToArray()
                                                })
                                                .ToDictionary(x => x.Key, x => x.Values);
            }
            if (exception is InvalidDiscordUserException discordex)
            {
                errors = new Dictionary<string, string[]>()
                {
                    { "DiscordIssue",new string[]
                        {
                            discordex.DiscordGuildId.ToString(),
                            discordex.DiscordUserId.ToString(),
                            discordex.Message
                        }
                    }
                };
            }
            if (exception is OAuthTokenRequestException oauthex)
            {
                errors = new Dictionary<string, string[]>()
                {
                    { oauthex.error,new string[]
                        {
                            oauthex.Message,
                        }
                    }
                };
            }
            if (exception is InvalidEntityOperationException entityEx)
            {
                errors = new Dictionary<string, string[]>()
                {
                    { "EntityOperation",new string[]
                        {
                            $"{entityEx.EntityId}",
                            $"{entityEx.EntityType}",
                            $"{entityEx.Message}"
                        }
                    }
                };
            }
            return errors;
        }
    }
}

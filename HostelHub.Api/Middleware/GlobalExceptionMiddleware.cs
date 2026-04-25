using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using FluentValidation;

namespace HostelHub.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";
        
        var statusCode = (int)HttpStatusCode.InternalServerError;
        if (exception is UnauthorizedAccessException) statusCode = (int)HttpStatusCode.Unauthorized;
        else if (exception is ArgumentException) statusCode = (int)HttpStatusCode.BadRequest;
        else if (exception is ValidationException) statusCode = (int)HttpStatusCode.BadRequest;
        
        context.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = exception is ValidationException ? "Validation Failed" : "An error occurred while processing your request.",
            Detail = exception is ValidationException 
                ? "One or more validation errors occurred." 
                : exception.Message, // CAUTION: Only expose generic message in production
            Instance = context.Request.Path
        };

        if (exception is ValidationException validationException)
        {
             var errors = validationException.Errors
                 .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                 .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
             
             problemDetails.Extensions.Add("errors", errors);
        }

        var json = JsonSerializer.Serialize(problemDetails);
        return context.Response.WriteAsync(json);
    }
}

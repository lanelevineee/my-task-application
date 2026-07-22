using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, problemDetails) = exception switch
        {
            ValidationException validationEx =>
                (HttpStatusCode.BadRequest, CreateProblemDetails(
                    "Validation Error",
                    (int)HttpStatusCode.BadRequest,
                    validationEx.Message,
                    validationEx.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }))),

            KeyNotFoundException keyNotFoundEx =>
                (HttpStatusCode.NotFound, CreateProblemDetails(
                    "Not Found",
                    (int)HttpStatusCode.NotFound,
                    keyNotFoundEx.Message)),

            UnauthorizedAccessException =>
                (HttpStatusCode.Unauthorized, CreateProblemDetails(
                    "Unauthorized",
                    (int)HttpStatusCode.Unauthorized,
                    "You are not authorized to perform this action.")),

            _ =>
                (HttpStatusCode.InternalServerError, CreateProblemDetails(
                    "Internal Server Error",
                    (int)HttpStatusCode.InternalServerError,
                    "An unexpected error occurred. Please try again later."))
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
    }

    private static ProblemDetails CreateProblemDetails(string title, int status, string detail, object? errors = null)
    {
        var problemDetails = new ProblemDetails
        {
            Title = title,
            Status = status,
            Detail = detail,
            Instance = null
        };

        if (errors != null)
        {
            problemDetails.Extensions["errors"] = errors;
        }

        return problemDetails;
    }
}
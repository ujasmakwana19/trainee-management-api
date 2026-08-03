using System.Formats.Asn1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using TraineeManagement.WebCommons.ErrorCodesUtils;
using TraineeManagement.WebCommons.ExceptionUtils;
using RabbitMQ.Client.Exceptions;
using StackExchange.Redis;

namespace TraineeManagement.WebCommons.ExceptionMiddlewares;

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
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Not found: {Message}", ex.Message);
            await WriteResponse(context, StatusCodes.Status404NotFound, ex._code, ex._message);
        }
        catch (UnauthorizedException ex)
        {
            _logger.LogWarning("Unauthorized: {Message}", ex.Message);
            await WriteResponse(context, StatusCodes.Status401Unauthorized, ex._code, ex._message);
        }
        catch (ForbiddenException ex)
        {
            _logger.LogWarning("Unauthorized User Forbidden: {Message}", ex.Message);
            await WriteResponse(context, StatusCodes.Status403Forbidden, ex._code, ex._message);
        }
        catch (BadRequestException ex)
        {
            _logger.LogWarning("Bad request: {Message}", ex.Message);
            await WriteResponse(context, StatusCodes.Status400BadRequest, ex._code, ex._message);
        }
        catch (JwtOperationException ex)
        {
            _logger.LogError("JWT Invalid operation: {Message}", ex.Message);
            await WriteResponse(context, StatusCodes.Status500InternalServerError, 
                ErrorCodes.JWT_OPERATION_FAILED.Code,
                ErrorCodes.JWT_OPERATION_FAILED.Message);
        }
        catch (ServerCredentialException ex)
        {
            _logger.LogError(ex, "Server credential error: {Message}", ex.Message);
            await WriteResponse(context, StatusCodes.Status500InternalServerError, 
                ErrorCodes.SERVER_CREDENTIAL_FAILED.Code,
                ErrorCodes.SERVER_CREDENTIAL_FAILED.Message);
        }
        catch (QueuingOperationExeception ex)
        {
            _logger.LogError("Queuing operation error: {Message}", ex);
            await WriteResponse(context, StatusCodes.Status500InternalServerError, 
                ex._code,
                ex._message);
        }
        catch (InterServiceOperationExeception ex)
        {
            await WriteResponse(context, StatusCodes.Status500InternalServerError, 
                ex._code,
                ex._message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError("Invalid operation or Credentials error: {Message}", ex.Message);
            await WriteResponse(context, StatusCodes.Status500InternalServerError, 
                ErrorCodes.SERVER_CREDENTIAL_FAILED.Code,
                ErrorCodes.SERVER_CREDENTIAL_FAILED.Message);
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogError("Unable to Connect to Redis: {Message}", ex.Message);
            await WriteResponse(context, StatusCodes.Status500InternalServerError, 
                ErrorCodes.REDIS_ERROR.Code,
                ErrorCodes.REDIS_ERROR.Message);
        }
        catch (RabbitMQClientException ex)
        {
            _logger.LogError("Unable to Connect to RabbitMq: {Message}", ex.Message);
            await WriteResponse(context, StatusCodes.Status500InternalServerError, 
                ErrorCodes.RABBIT_MQ_ERROR.Code,
                ErrorCodes.RABBIT_MQ_ERROR.Message);
        }
        catch (DataBaseOperationFailed ex)
        {
            // First check if the wrapped exception contains a specific MySQL constraint failure
            if (await HandleMySqlExceptionAsync(context, ex._ex))
            {
                return;
            }

            // Fallback for general database operation failures
            _logger.LogError(ex._ex, "Failed to Perform Db Operation: {Message}", ex.Message);
            await WriteResponse(context, StatusCodes.Status400BadRequest, ex._code, ex._message);
        }
        catch (Exception ex)
        {
            if (await HandleMySqlExceptionAsync(context, ex))
            {
                return;
            }

            _logger.LogError(ex, "Unhandled exception on {Method} {Path} {TypeOfException}",
                context.Request.Method, context.Request.Path, ex.GetType().Name);
                
            await WriteResponse(context, StatusCodes.Status500InternalServerError, 
                ErrorCodes.SERVER_ERROR.Code, ErrorCodes.SERVER_ERROR.Message);
        }
    }

    private async Task<bool> HandleMySqlExceptionAsync(HttpContext context, Exception ex)
    {
        MySqlException? mysqlEx = GetInnermostException<MySqlException>(ex);
        if (mysqlEx == null) return false;

        _logger.LogError("MySQL Error Code: {Number} | Message: {Message}", mysqlEx.Number, mysqlEx.Message);

        switch (mysqlEx.Number)
        {
            case 1062: // Unique constraint violation (Duplicate email/username)
                await WriteResponse(context, StatusCodes.Status409Conflict, 
                    ErrorCodes.UNIQUE_USER.Code,
                    ErrorCodes.UNIQUE_USER.Message);
                return true;

            case 1451: // Foreign key constraint failure on Delete
                _logger.LogWarning("Foreign key constraint failure on Delete: {Message}", mysqlEx.Message);
                await WriteResponse(context, StatusCodes.Status400BadRequest, 
                    ErrorCodes.DELETE_RESTRICT_REFERENCE.Code,
                    ErrorCodes.DELETE_RESTRICT_REFERENCE.Message);
                return true;

            case 1452: // Foreign key constraint failure on Insert/Update
                _logger.LogWarning("Foreign key constraint failure on Insert or Update: {Message}", mysqlEx.Message);
                await WriteResponse(context, StatusCodes.Status400BadRequest, 
                    ErrorCodes.REFERENCE_NOT_EXISTS.Code,
                    ErrorCodes.REFERENCE_NOT_EXISTS.Message);
                return true;

            default:
                return false;
        }
    }

    private static TException? GetInnermostException<TException>(Exception? ex) where TException : Exception
    {
        while (ex != null)
        {
            if (ex is TException typedEx)
                return typedEx;

            ex = ex.InnerException;
        }
        return null;
    }

    private static async Task WriteResponse(HttpContext context, int statusCode, int code, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { success = false, message = message, errorCode = code });
    }
}
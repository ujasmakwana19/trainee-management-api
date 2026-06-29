using System.Formats.Asn1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using TraineeManagement.Contracts.ErrorCodesUtils;
using TraineeManagement.Contracts.ExceptionUtils;
namespace TraineeManagement.Contracts.ExceptionMiddlewares;
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
            _logger.LogWarning("Not found: {Message}", ex);
            await WriteResponse(context, StatusCodes.Status404NotFound, ex._code, ex._message);
        }
        catch (UnauthorizedException ex)
        {
            _logger.LogWarning("Unauthorized: {Message}", ex);
            await WriteResponse(context, StatusCodes.Status401Unauthorized, ex._code, ex._message);
        }
        catch (BadRequestException ex)
        {
            _logger.LogWarning("Bad request: {Message}", ex);
            await WriteResponse(context, StatusCodes.Status400BadRequest, ex._code, ex._message);
        }
        catch (JwtOperationException ex)
        {
            _logger.LogError("Invalid operation: {Message}", ex);
            await WriteResponse(context, StatusCodes.Status500InternalServerError, 
            ErrorCodes.JWT_OPERATION_FAILED.Code,
            ErrorCodes.JWT_OPERATION_FAILED.Message);
        }
        catch (ServerCredentialException ex)
        {
            _logger.LogError("Server credential error: {Message}", ex);
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
        catch (BrokenCircuitException ex)
        {
            // Circuit is open — we didn't even attempt the call, we already know
            // the dependency is unhealthy. This is the "fallback behaviour" path.
            _logger.LogWarning(ex, "Circuit breaker open on {Method} {Path}",
                context.Request.Method, context.Request.Path);
            await WriteResponse(context, StatusCodes.Status503ServiceUnavailable,
                ErrorCodes.SERVICE_UNAVAILABLE.Code, ErrorCodes.SERVICE_UNAVAILABLE.Message);
        }
        catch (TimeoutRejectedException ex)
        {
            // Per-attempt or total resilience timeout exhausted — dependency is
            // slow/unresponsive, not a bug in our code.
            _logger.LogWarning(ex, "Upstream timeout on {Method} {Path}",
                context.Request.Method, context.Request.Path);
            await WriteResponse(context, StatusCodes.Status503ServiceUnavailable,
                ErrorCodes.SERVICE_UNAVAILABLE.Code, ErrorCodes.SERVICE_UNAVAILABLE.Message);
        }
        catch (HttpRequestException ex)
        {
            // Network-level failure (connection refused, DNS, reset) reaching
            // the dependency directly, outside the resilience pipeline's retry
            // budget (e.g. circuit already half-open and this probe failed).
            _logger.LogWarning(ex, "Upstream unreachable on {Method} {Path}",
                context.Request.Method, context.Request.Path);
            await WriteResponse(context, StatusCodes.Status503ServiceUnavailable,
                ErrorCodes.SERVICE_UNAVAILABLE.Code, ErrorCodes.SERVICE_UNAVAILABLE.Message);
        }
        catch (Exception ex)
        {
            if(ex.InnerException is MySqlException mysqlEx){
                _logger.LogError($"ERROR CODE::::{mysqlEx.Number}::::::::::\t\t{ex}");
                if(mysqlEx.Number == 1451) // Foreign key constraint failure
                {
                    _logger.LogWarning("Foreign key constraint failure on Delete: {Message}", mysqlEx.Message);
                    await WriteResponse(context,StatusCodes.Status400BadRequest, 
                    ErrorCodes.DELETE_RESTRICT_REFERENCE.Code,
                    ErrorCodes.DELETE_RESTRICT_REFERENCE.Message);
                }
                if(mysqlEx.Number == 1452) // Foreign key constraint failure on insert or update
                {
                    _logger.LogWarning("Foreign key constraint failure on Insert or Update: {Message}", mysqlEx.Message);
                    await WriteResponse(context,StatusCodes.Status400BadRequest, 
                    ErrorCodes.REFERENCE_NOT_EXISTS.Code,
                    ErrorCodes.REFERENCE_NOT_EXISTS.Message);
                }
                if(mysqlEx.Number == 1062) // Foreign key constraint failure on insert or update
                {
                    await WriteResponse(context,StatusCodes.Status400BadRequest, 
                    ErrorCodes.UNIQUE_USERNAME.Code,
                    ErrorCodes.UNIQUE_USERNAME.Message);
                }
                    
            }
            else{
                _logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                    context.Request.Method, context.Request.Path);
                await WriteResponse(context,StatusCodes.Status500InternalServerError, 
                ErrorCodes.SERVER_ERROR.Code, ErrorCodes.SERVER_ERROR.Message);
            }
        }
    }
    private static async Task WriteResponse(HttpContext context, int statusCode, int code,string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { success = false ,message = message , errorCode = code});
    }
}

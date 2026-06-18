using System.Formats.Asn1;
using MySqlConnector;
using TraineeManagement.Api.ExceptionUtils;
namespace TraineeManagement.Api.ExceptionMiddlewares;
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
            await WriteResponse(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (UnauthorizedException ex)
        {
            _logger.LogWarning("Unauthorized: {Message}", ex.Message);
            await WriteResponse(context, StatusCodes.Status401Unauthorized, ex._code, ex._message);
        }
        catch (BadRequestException ex)
        {
            _logger.LogWarning("Bad request: {Message}", ex.Message);
            await WriteResponse(context, StatusCodes.Status400BadRequest, ex._code, ex._message);
        }
        catch (JwtOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation: {Message}", ex.Message);
            await WriteResponse(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred while processing authentication please retry");
        }
        catch (Exception ex)
        {
            if(ex.InnerException is MySqlException mysqlEx){
                _logger.LogError($"ERROR CODE::::{mysqlEx.Number}::::::::::");
                if(mysqlEx.Number == 1451) // Foreign key constraint failure
                {
                    _logger.LogWarning("Foreign key constraint failure on Delete: {Message}", mysqlEx.Message);
                    await WriteResponse(context,StatusCodes.Status400BadRequest, "Cannot delete or update because of related data. Please remove related data first or change reference");
                }
                if(mysqlEx.Number == 1452) // Foreign key constraint failure on insert or update
                {
                    _logger.LogWarning("Foreign key constraint failure on Insert or Update: {Message}", mysqlEx.Message);
                    await WriteResponse(context,StatusCodes.Status400BadRequest, "Related data not found, Please ensure referenced data exists..");
                }
                if(mysqlEx.Number == 1062) // Foreign key constraint failure on insert or update
                {
                    await WriteResponse(context,StatusCodes.Status400BadRequest, "Username Already Exists");
                }
                    
            }
            else{
                _logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                    context.Request.Method, context.Request.Path);
                await WriteResponse(context,StatusCodes.Status500InternalServerError, "Something Went Wrong, Please Try Again");
            }
        }
    }

    private static async Task WriteResponse(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { message = message});
    }
    private static async Task WriteResponse(HttpContext context, int statusCode, int code,string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { message = message , errorCode = code});
    }
}

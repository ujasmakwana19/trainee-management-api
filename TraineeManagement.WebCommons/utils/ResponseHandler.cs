using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.WebCommons.ErrorCodesUtils;
namespace TraineeManagement.WebCommons.ResponseHandlerUtil;

public static class ResponseHandler
{
    public static ActionResult SuccessResponse(HttpContext context, ErrorCode errorCode, object value)
    {
        object body = new
        {
            Success = true,
            ErrorMessage = errorCode.Message,
            ErrorCode = errorCode.Code,
            data = value
        };
        return new ObjectResult(body)
        {
            StatusCode = StatusCodes.Status200OK
        };
        
    }
    public static ActionResult AcceptResponse(HttpContext context, ErrorCode errorCode, object value)
    {
        object body = new
        {
            Success = true,
            ErrorMessage = errorCode.Message,
            ErrorCode = errorCode.Code,
            data = value
        };
        return new ObjectResult(body)
        {
            StatusCode = StatusCodes.Status202Accepted
        };
        
    }

    public static ActionResult CreateResponse(int statusCode, ErrorCode errorCode, bool success = false)
    {
        object body = new
        {
            Success = success,
            message = errorCode.Message,
            errorCode = errorCode.Code
        };

        return new ObjectResult(body)
        {
            StatusCode = statusCode
        };
    }
}
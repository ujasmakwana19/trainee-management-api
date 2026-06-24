using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.ErrorCodesUtils;
namespace TraineeManagement.Api.ResponseHandlerUtil;

public static class ResponseHandler
{
    public static ActionResult SuccessResponse(HttpContext context, ErrorCode errorCode, object value)
    {
        object body = new
        {
            ErrorMessage = errorCode.Message,
            ErrorCode = errorCode.Code,
            data = value
        };
        return new ObjectResult(body)
        {
            StatusCode = StatusCodes.Status200OK
        };
        
    }

    public static ActionResult CreateResponse(int statusCode, ErrorCode errorCode)
    {
        object body = new
        {
            message = errorCode.Message,
            errorCode = errorCode.Code
        };

        return new ObjectResult(body)
        {
            StatusCode = statusCode
        };
    }
}
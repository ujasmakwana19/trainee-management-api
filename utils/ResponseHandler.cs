using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.ErrorCodesUtils;

namespace TraineeManagement.Api.ResponseHandlerUtil;

public static class ResponseHandler
{
    public static ActionResult SuccessResponse(HttpContext context, object value, int ErrorCode)
    {
        object body = new
        {
            data = value,
            ErrorCode = ErrorCode
        };
        return new ObjectResult(body)
        {
            StatusCode = StatusCodes.Status200OK
        };
        
    }

    public static ActionResult CreateResponse(HttpContext context, int statusCode, int ErrorCode, Dictionary<string, string[]> errorsArray)
    {
        object body = new
        {
            errorCode = ErrorCode,
            errors = errorsArray
        };

        return new ObjectResult(body)
        {
            StatusCode = statusCode
        };
    }
}
using System.Reflection.Metadata;
using TraineeManagement.Api.ErrorMessageUtils;
namespace TraineeManagement.Api.ErrorCodesUtils;
public record ErrorCode(int Code, string Message);

public static class ErrorCodes
{
    public static readonly ErrorCode SUCCESS = new ErrorCode(2000, ErrorMessage.Success);
    public static readonly ErrorCode INVALID_MODEL = new ErrorCode(4000, ValidationErrorMessage.InvalidInput);

    public static readonly ErrorCode INVALID_CREDENTIALS = new ErrorCode(4001, ErrorMessage.InvalidCredentials);
    public static readonly ErrorCode INVALID_PARAMS = new ErrorCode(4000, ValidationErrorMessage.InvalidInputParamsQuery);
}
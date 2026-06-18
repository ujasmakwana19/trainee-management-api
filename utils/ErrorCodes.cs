using System.Reflection.Metadata;
using TraineeManagement.Api.ErrorMessageUtils;
namespace TraineeManagement.Api.ErrorCodesUtils;
public record ErrorCode(int Code, string Message);

public static class ErrorCodes
{
    public static readonly ErrorCode SUCCESS = new ErrorCode(2000, ErrorMessage.Success);
    public static readonly ErrorCode INVALID_MODEL = new ErrorCode(4000, ValidationErrorMessage.InvalidInput);

    public static readonly ErrorCode INVALID_CREDENTIALS = new ErrorCode(4001, ErrorMessage.InvalidCredentials);
    public static readonly ErrorCode INVALID_PARAMS_QUERY = new ErrorCode(4000, ValidationErrorMessage.InvalidInputParamsQuery);
    public static readonly ErrorCode NOT_FOUND_TRAINEE = new ErrorCode(4040, ErrorMessage.TraineeNotFound);
    public static readonly ErrorCode NOT_FOUND_MENTOR = new ErrorCode(4040, ErrorMessage.MentorNotFound);
    public static readonly ErrorCode NOT_FOUND_TASK = new ErrorCode(4040, ErrorMessage.TaskNotFound);
    public static readonly ErrorCode NOT_FOUND_TASK_ASSIGNMENT = new ErrorCode(4040, ErrorMessage.TaskAssignementNotFound);
    public static readonly ErrorCode NOT_FOUND_SUBMISSION = new ErrorCode(4040, ErrorMessage.SubmissionNotFound);
    public static readonly ErrorCode NOT_FOUND_REVIEW = new ErrorCode(4040, ErrorMessage.ReviewNotFound);
    public static readonly ErrorCode REFERENCE_NOT_EXISTS = new ErrorCode(4040, ErrorMessage.ReferenceInvalid);
    public static readonly ErrorCode DELETE_RESTRICT_REFERENCE = new ErrorCode(4000, ErrorMessage.DeleteNotPossibleReferenceExists);
    public static readonly ErrorCode UNIQUE_USERNAME = new ErrorCode(4000, ErrorMessage.UniqueUsername);
    public static readonly ErrorCode JWT_OPERATION_FAILED = new ErrorCode(5000, ErrorMessage.UniqueUsername);
    public static readonly ErrorCode SERVER_ERROR = new ErrorCode(5000, ErrorMessage.UniqueUsername);
}
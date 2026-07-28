using TraineeManagement.WebCommons.ErrorMessageUtils;
namespace TraineeManagement.WebCommons.ErrorCodesUtils;
public record ErrorCode(int Code, string Message);

public static class ErrorCodes
{
    public static readonly ErrorCode SUCCESS = new ErrorCode(2000, ErrorMessage.Success);
    public static readonly ErrorCode ACCEPTED = new ErrorCode(2020, ErrorMessage.Queued);
    public static readonly ErrorCode INVALID_MODEL = new ErrorCode(4000, ErrorMessage.InvalidInput);
    public static readonly ErrorCode INVALID_FILE = new ErrorCode(4000, ErrorMessage.InvalidFile);
    public static readonly ErrorCode CONTENT_TOO_LARGE = new ErrorCode(4130, ErrorMessage.ContentTooLarge);

    public static readonly ErrorCode INVALID_CREDENTIALS = new ErrorCode(4001, ErrorMessage.InvalidCredentials);
    public static readonly ErrorCode INVALID_TOKEN = new ErrorCode(4001, ErrorMessage.InvalidToken);
    public static readonly ErrorCode INVALID_PARAMS_QUERY = new ErrorCode(4000, ErrorMessage.InvalidInputParamsQuery);
    public static readonly ErrorCode SESSION_EXPIRED = new ErrorCode(4010, ErrorMessage.SessionExpired);
    public static readonly ErrorCode UNAUTHORISE_ACCESS = new ErrorCode(4010, ErrorMessage.UnauthoriseAccess);

    public static readonly ErrorCode ROLE_FORBIDDEN = new ErrorCode(4030, ErrorMessage.RoleForbidden);
    public static readonly ErrorCode NOT_OWNER_ACCESS = new ErrorCode(4010, ErrorMessage.NotOwnerOfResource);
    
    public static readonly ErrorCode NOT_FOUND_TRAINEE = new ErrorCode(4040, ErrorMessage.TraineeNotFound);
    public static readonly ErrorCode NOT_FOUND_MENTOR = new ErrorCode(4040, ErrorMessage.MentorNotFound);
    public static readonly ErrorCode NOT_FOUND_TASK = new ErrorCode(4040, ErrorMessage.TaskNotFound);
    public static readonly ErrorCode NOT_FOUND_TASK_ASSIGNMENT = new ErrorCode(4040, ErrorMessage.TaskAssignementNotFound);
    public static readonly ErrorCode NOT_FOUND_SUBMISSION = new ErrorCode(4040, ErrorMessage.SubmissionNotFound);
    public static readonly ErrorCode NOT_FOUND_REVIEW = new ErrorCode(4040, ErrorMessage.ReviewNotFound);
    public static readonly ErrorCode NOT_FOUND_FILE = new ErrorCode(4040, ErrorMessage.FileNotFound);
    public static readonly ErrorCode NOT_FOUND_QUEUEMESSAGE = new ErrorCode(4040, ErrorMessage.QueueMessageNotFound);
 
 
    public static readonly ErrorCode REFERENCE_NOT_EXISTS = new ErrorCode(4040, ErrorMessage.ReferenceInvalid);
    public static readonly ErrorCode DELETE_RESTRICT_REFERENCE = new ErrorCode(4000, ErrorMessage.DeleteNotPossibleReferenceExists);
    public static readonly ErrorCode UNIQUE_USERNAME = new ErrorCode(4000, ErrorMessage.UniqueUsername);
 
 
    public static readonly ErrorCode JWT_OPERATION_FAILED = new ErrorCode(5000, ErrorMessage.UniqueUsername);
    public static readonly ErrorCode SERVER_ERROR = new ErrorCode(5000, ErrorMessage.ServerError);
    public static readonly ErrorCode SERVER_CREDENTIAL_FAILED = new ErrorCode(5001, ErrorMessage.ServerError);
    public static readonly ErrorCode INTER_SERVICE_FAILED = new ErrorCode(5001, ErrorMessage.InterServiceOperationFailed);
    public static readonly ErrorCode REDIS_ERROR = new ErrorCode(5001, ErrorMessage.RedisError);
    public static readonly ErrorCode RABBIT_MQ_ERROR = new ErrorCode(5001, ErrorMessage.RabbitMqError);
    public static readonly ErrorCode QUEUING_OPERATION_FAILED = new ErrorCode(5001, ErrorMessage.QueueOperationFailed);
}
// namespace TraineeManagement.Api.ErrorMessageUtil;

// public static class Res
// // "Username must be between 3 and 50 characters.
// // "Invalid email address format."
// // "Role must be Admin, Mentor, or Trainee."
namespace TraineeManagement.WebCommons.ErrorMessageUtils;

public static class ErrorMessage
{
    
    public const string InvalidInput          = "One or more Invalid input values or format.";
    public const string InvalidValue          = "Invalid value provided.";
    public const string InvalidFile           = "Invalid files provided or files format not supported.";
    public const string InvalidInputParamsQuery          = "Invalid value or type provided.";
    public const string UnauthoriseAccess                    = "Unauthorised access to resource";
    public const string ContentTooLarge                    = "Content is too large";
    public const string ValidationErrorOccured             = "One or more validation errors occurred.";
    public const string Success                            = "Success";
    public const string Queued                            = "Task will be processed";
    public const string InvalidCredentials                 = "Invalid credentials.";
    public const string InvalidToken                 = "Unauthorised, Please login again";
    public const string TraineeNotFound                    = "Trainee Not Found";
    public const string MentorNotFound                    = "Mentor Not Found";
    public const string TaskNotFound                    = "Task Not Found";
    public const string TaskAssignementNotFound                    = "Task Assignment Not Found";
    public const string SubmissionNotFound                    = "Submission Not Found";
    public const string ReviewNotFound                    = "Review Not Found";
    public const string FileNotFound                    = "File Not Found";
    public const string QueueMessageNotFound                    = "Queue Message Not Found";
    public const string ReferenceInvalid                    = "Please Enter valid references";
    public const string DeleteNotPossibleReferenceExists                    = "Cannot Delete the referenced used values";
    public const string UniqueUsername                    = "Username Already Exists";
    public const string JwtError                        = "An UnExpected Error Occured , Please try again";
    public const string ServerError                        = "Something Went Wrong";
    public const string InterServiceOperationFailed         = "Some Services are not available, Please try again later";
    public const string QueueOperationFailed                        = "Something Went Wrong, while processing in background";

}
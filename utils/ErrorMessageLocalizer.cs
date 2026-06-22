// namespace TraineeManagement.Api.ErrorMessageUtil;

// public static class Res
// // "Username must be between 3 and 50 characters.
// // "Invalid email address format."
// // "Role must be Admin, Mentor, or Trainee."
namespace TraineeManagement.Api.ErrorMessageUtils;
public static class ValidationErrorMessage
{
    public const string Required                = "{0} is required.";
    public const string StringLength            = "{0} must be atleast {1} characters.";
    public const string StringLengthRange       = "{0} must be between {2} and {1} characters.";
    public const string Email                   = "Must be a valid email address.";

    public const string EnumDataType            = "{0} must be a valid value from the provided.";
    public const string StartDateAfterEndDate   = "Start date must be before end date.";

    // Serializatio
    public const string InvalidInput          = "One or more Invalid input values or format.";
    public const string InvalidValue          = "Invalid value provided.";
    public const string InvalidFile           = "Invalid files provided or files format not supported.";
    public const string InvalidInputParamsQuery          = "Invalid value or type provided.";
}
public static class ErrorMessage
{
    public const string ValidationErrorOccured             = "One or more validation errors occurred.";
    public const string Success                            = "Success";
    public const string InvalidCredentials                 = "Invalid credentials.";
    public const string InvalidToken                 = "Unauthorised, Please login again";
    public const string TraineeNotFound                    = "Trainee Not Found";
    public const string MentorNotFound                    = "Mentor Not Found";
    public const string TaskNotFound                    = "Task Not Found";
    public const string TaskAssignementNotFound                    = "Task Assignment Not Found";
    public const string SubmissionNotFound                    = "Submission Not Found";
    public const string ReviewNotFound                    = "Review Not Found";
    public const string ReferenceInvalid                    = "Please Enter valid references";
    public const string DeleteNotPossibleReferenceExists                    = "Cannot Delete the referenced used values";
    public const string UniqueUsername                    = "Username Already Exists";
    public const string JwtError                        = "An UnExpected Error Occured , Please try again";
    public const string ServerError                        = "Something Went Wrong";

}
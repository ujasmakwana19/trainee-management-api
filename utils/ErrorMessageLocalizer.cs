// namespace TraineeManagement.Api.ErrorMessageUtil;

// public static class Res
// // "Username must be between 3 and 50 characters.
// // "Invalid email address format."
// // "Role must be Admin, Mentor, or Trainee."
namespace TraineeManagement.Api.ErrorMessageUtils;
public static class ValidationErrorMessage
{
    public const string Required              = "{0} is required..";
    public const string StringLength          = "{0} must be atleast {1} characters.";
    public const string StringLengthRange          = "{0} must be between {2} and {1} characters.";
    public const string Email                      = "{0} must be a valid email address.";

    public const string EnumDataType                 = "{0} must be a valid value from the provided.";
    public const string StartDateAfterEndDate = "Start date must be before end date.";

    // Serialization / input errors — all collapsed into these
    public const string InvalidInput          = "Invalid input values or format.";
    public const string InvalidValue          = "Invalid value provided.";
    public const string ServerError           = "An unexpected error occurred.";
}
public static class ErrorMessage
{
    public const string ValidationErrorOccured              = "One or more validation errors occurred.";
    public const string Success                             = "Success";
    public const string InvalidCredentials                    = "Invalid credentials.";

}
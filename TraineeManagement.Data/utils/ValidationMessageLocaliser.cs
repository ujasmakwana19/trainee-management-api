namespace TraineeManagement.Data.ValidationConstants;
public static class ValidationErrorMessage
{
    public const string Required                = "{0} is required.";
    public const string StringLength            = "{0} must be atleast {1} characters.";
    public const string StringLengthRange       = "{0} must be between {2} and {1} characters.";
    public const string Email                   = "Must be a valid email address.";

    public const string EnumDataType            = "{0} must be a valid value from the provided.";
    public const string StartDateAfterEndDate   = "Start date must be before end date.";

    
}
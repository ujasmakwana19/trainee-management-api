using Microsoft.AspNetCore.Mvc.ModelBinding;
using TraineeManagement.Contracts.ErrorMessageUtils;
using System.Text.Json;

public static class UtilityHelper
{
    public static Dictionary<string, string[]> GetInvalidModelStateErrors(ModelStateDictionary modelState, string modelName)
    {
        Dictionary<string, string[]> annotationErrors = new Dictionary<string, string[]>();
        bool hasSerializationError = false;

        foreach (KeyValuePair<string, ModelStateEntry> kvp in modelState)
        {
            if (kvp.Value == null || kvp.Value.Errors.Count == 0)
                continue;

            // Serialization errors land with "$" keys or JsonException
            bool isSerializationError = kvp.Key.StartsWith("$") ||
                kvp.Key == modelName ||
                kvp.Value.Errors.Any(e => e.Exception is JsonException);

            if (isSerializationError)
            {
                hasSerializationError = true;
                continue;
            }

            annotationErrors[kvp.Key] = kvp.Value.Errors
                .Select(e => !string.IsNullOrEmpty(e.ErrorMessage)
                    ? e.ErrorMessage
                    : ValidationErrorMessage.InvalidValue)
                .ToArray();
        }

        // All serialization errors into one clean entry
        if (hasSerializationError)
            annotationErrors["body"] = new[] { ValidationErrorMessage.InvalidInput };

        return annotationErrors;
    }

    

    public static Dictionary<string, string[]> GetInvalidParamsQuery(ModelStateDictionary modelState)
    {
        Dictionary<string, string[]> annotationErrors = new Dictionary<string, string[]>();

        List<string> errorsList = new List<string>();
        foreach (KeyValuePair<string, ModelStateEntry> kvp in modelState)
        {
            if (kvp.Value == null || kvp.Value.Errors.Count == 0)
                continue;


            errorsList.AddRange(kvp.Value.Errors
                .Select(e => !string.IsNullOrEmpty(e.ErrorMessage)
                    ? e.ErrorMessage
                    : ValidationErrorMessage.InvalidValue));

        }
        annotationErrors["body"] = errorsList.ToArray();
        return annotationErrors;
    }

}

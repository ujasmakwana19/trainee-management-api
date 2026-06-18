using Microsoft.AspNetCore.Mvc.ModelBinding;
using TraineeManagement.Api.ErrorMessageUtils;
using System.Text.Json;
using Namotion.Reflection;

public static class UtilityHelper
{
    public static Dictionary<string, string[]> GetInvalidModelStateErrors(ModelStateDictionary modelState, string modelName)
    {
        System.Console.WriteLine(modelName);
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

        // Collapse all serialization errors into one clean entry
        if (hasSerializationError)
            annotationErrors["body"] = new[] { ValidationErrorMessage.InvalidInput };

        return annotationErrors;
    }

    public static long isValidTypeLong(string value)
    {
        if(value is null)
            return 0;

        if (!long.TryParse(value, out long parsedValue) || parsedValue < 1)
        {
            return 0;
        }
        return parsedValue;
    }


}

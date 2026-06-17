using Microsoft.AspNetCore.Mvc.ModelBinding;

public static class UtilityHelper
{
    public static Dictionary<string, string[]> GetInvalidModelStateErrors(ModelStateDictionary modelState)
    {
        return modelState
            .Where(x => x.Value != null && x.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key, 
                kvp => kvp.Value!.Errors
                    .Select(e => !string.IsNullOrEmpty(e.ErrorMessage) 
                        ? e.ErrorMessage 
                        : e.Exception?.Message ?? "Invalid value provided.")
                    .ToArray() 
            );
    }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TraineeManagement.Contracts.ErrorMessageUtils;
using TraineeManagement.Data.ValidationConstantUtils;

namespace TraineeManagement.Api.FileAttributeCustom;
// Used this to disable by default binding of the stream to the asp dotnet internal
// because if we use the stream to handle the files and if we donot immediately 
// take the stream , then When your code pauses to await the database check 
//  the HTTP pipeline can treat the request body stream as abandoned, or 
// the client finishes transmitting but nothing collects the bytes, causing the 
// underlying TCP socket to truncate or flush early. 
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class DisableFormValueModelBindingAttribute : Attribute, IResourceFilter
{
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        // This completely unhooks the default form readers so ASP.NET 
        // doesn't touch Request.Body while your code is running.
        IList<IValueProviderFactory> factories = context.ValueProviderFactories;
        factories.RemoveType<FormValueProviderFactory>();
        factories.RemoveType<FormFileValueProviderFactory>();
        factories.RemoveType<JQueryFormValueProviderFactory>();
    }

    public void OnResourceExecuted(ResourceExecutedContext context)
    {
    }
}
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TraineeManagement.Api.ErrorMessageUtils;
using TraineeManagement.Api.ValidationConstantUtils;
public class RequiredFieldAttribute : RequiredAttribute
{
    public RequiredFieldAttribute()
    {
        ErrorMessage = ValidationErrorMessage.Required;
    }
}

// Attributes/StringLengthFieldAttribute.cs
public class StringLengthFieldAttribute : StringLengthAttribute
{
    public StringLengthFieldAttribute(int max) : base(ValidationConstant.MAX_LENTH_GENERIC_INPUT)
    {
        ErrorMessage = ValidationErrorMessage.StringLength;
    }

    public StringLengthFieldAttribute(int max, int min) : base(ValidationConstant.MAX_LENTH_GENERIC_INPUT)
    {
        MinimumLength = min;
        ErrorMessage  = ValidationErrorMessage.StringLengthRange;
    }
}
public class EmailFieldAttribute : ValidationAttribute
{
    public EmailFieldAttribute() 
    {
        ErrorMessage = ValidationErrorMessage.Email;
    }

    public override bool IsValid(object? value)
    {
        if (value == null) return true; 

        String? email = value.ToString();
        
        // reuse the sealed attribute's logic internally don't inherit it
        EmailAddressAttribute emailValidator = new EmailAddressAttribute();
        return emailValidator.IsValid(email);
    }
}

public class EnumDataTypeFieldAttribute : ValidationAttribute
{
    public Type EnumType { get; }
    public EnumDataTypeFieldAttribute(Type enumType) 
    {
        EnumType = enumType;
        ErrorMessage = ValidationErrorMessage.EnumDataType;
    }

    public override bool IsValid(object? value)
    {
        if (value == null) return true; 

        String? enumValue = value.ToString();
        
        // reuse the sealed attribute's logic internally don't inherit it
        EnumDataTypeAttribute enumValidator = new EnumDataTypeAttribute(this.EnumType);
        return enumValidator.IsValid(enumValue);
    }

}

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
        var factories = context.ValueProviderFactories;
        factories.RemoveType<FormValueProviderFactory>();
        factories.RemoveType<FormFileValueProviderFactory>();
        factories.RemoveType<JQueryFormValueProviderFactory>();
    }

    public void OnResourceExecuted(ResourceExecutedContext context)
    {
    }
}
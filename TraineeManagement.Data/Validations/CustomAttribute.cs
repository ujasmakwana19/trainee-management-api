using System.ComponentModel.DataAnnotations;
using TraineeManagement.Data.ValidationConstants;
using TraineeManagement.Data.ValidationConstantUtils;
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

using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.MentorModel;
using TraineeManagement.Api.ValidationConstantUtils;
namespace TraineeManagement.Api.MentorDTO;

public record MentorRequestBody
(
    [RequiredField]
    [StringLengthField(ValidationConstant.MAX_LENTH_NAME_INPUT, 
    MinimumLength = ValidationConstant.MIN_LENTH_GENERIC_INPUT)]
    String FirstName,

    [RequiredField]
    [StringLengthField(ValidationConstant.MAX_LENTH_NAME_INPUT,
    MinimumLength = ValidationConstant.MIN_LENTH_GENERIC_INPUT)]
    String LastName,

    [RequiredField]
    [EmailField]
    String Email,

    [RequiredField]
    [StringLengthField(ValidationConstant.MAX_LENTH_GENERIC_INPUT,
    MinimumLength = ValidationConstant.MIN_LENTH_GENERIC_INPUT)]
    String Expertise,

    [Required]
    [EnumDataTypeField(typeof(MentorStatus))]
    MentorStatus Status
);

public record MentorResponse(
    long Id,
    String FirstName,
    String LastName,
    String Email,
    String Expertise,
    MentorStatus Status
);
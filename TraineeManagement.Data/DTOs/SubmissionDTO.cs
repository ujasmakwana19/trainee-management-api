using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.SubmissionModel;
using TraineeManagement.Api.ValidationConstantUtils;

namespace TraineeManagement.Api.SubmissionDTO;

public record SubmissionRequestBody (
    [RequiredField]
    long TaskAssignmentId,

    [RequiredField]
    [StringLengthField(ValidationConstant.MAX_LENTH_GENERIC_INPUT , 
    MinimumLength = ValidationConstant.MIN_LENTH_GENERIC_INPUT)]
    string SubmissionUrl,

    [RequiredField]
    [StringLengthField(ValidationConstant.MAX_LENTH_GENERIC_INPUT , 
    MinimumLength = ValidationConstant.MIN_LENTH_GENERIC_INPUT)]
    string Notes,

    [RequiredField]
    DateOnly SubmittedDate,

    [RequiredField]
    [EnumDataTypeField(typeof(SubmissionStatus))]
    SubmissionStatus Status
);

public record SubmissionResponse(
    long Id,
    long TaskAssignmentId,
    string SubmissionUrl,
    string Notes,
    DateOnly SubmittedDate,
    SubmissionStatus Status
);

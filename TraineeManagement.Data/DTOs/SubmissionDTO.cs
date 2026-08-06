using System.ComponentModel.DataAnnotations;
using TraineeManagement.Data.SubmissionModel;
using TraineeManagement.Data.ValidationConstantUtils;

namespace TraineeManagement.Data.SubmissionDTO;

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

public record SubmissionPersonalResponse(
    long Id,
    long TaskAssignmentId,
    string TaskTitle,
    string SubmissionUrl,
    string Notes,
    DateOnly SubmittedDate,
    SubmissionStatus Status
);

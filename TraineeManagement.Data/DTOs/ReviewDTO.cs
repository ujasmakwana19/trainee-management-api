using System.ComponentModel.DataAnnotations;
using TraineeManagement.Data.ReviewModel;
using TraineeManagement.Data.ValidationConstantUtils;

public record ReviewRequestBody(
    [RequiredField]
    long SubmissionId,

    [RequiredField]
    long MentorId,

    [RequiredField]
    [StringLengthField(ValidationConstant.MAX_LENTH_GENERIC_INPUT, 
    MinimumLength = ValidationConstant.MIN_LENTH_GENERIC_INPUT)]
    string Feedback,

    long Score,

    [RequiredField]
    [EnumDataTypeField(typeof(ReviewStatusValue))]
    ReviewStatusValue ReviewStatus,

    [RequiredField]
    DateOnly ReviewedDate
);

public record ReviewResponse(
    long Id,
    long SubmissionId,
    long MentorId,
    string Feedback,
    long Score,
    ReviewStatusValue ReviewStatus,
    DateOnly ReviewedDate
);
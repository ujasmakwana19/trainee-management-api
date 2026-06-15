using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.ReviewModel;

public record ReviewRequestBody(
    [Required(ErrorMessage = "Submission is required")]
    long SubmissionId,

    [Required(ErrorMessage = "Reviewing Mentor is required")]
    long MentorId,

    [Required(ErrorMessage = "Mentor Feedback is required")]
    string Feedback,

    long Score,

    [Required]
    [AllowedValues(ReviewStatusValue.Accepted, ReviewStatusValue.ChangesRequired, ReviewStatusValue.Rejected, ErrorMessage = "Status must be Accepted, ChangesRequired or Rejected")]
    ReviewStatusValue ReviewStatus,

    [Required(ErrorMessage = "Review Date is required")]
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
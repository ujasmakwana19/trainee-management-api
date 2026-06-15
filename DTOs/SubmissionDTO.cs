using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.SubmissionModel;

namespace TraineeManagement.Api.SubmissionDTO;

public record SubmissionRequestBody (
    [Required(ErrorMessage = "Task Assignment Reference is Required")]
    long TaskAssignmentId,

    [Required(ErrorMessage = "Submission Url is Required")]
    string SubmissionUrl,

    [Required(ErrorMessage = "Notes are Required")]
    string Notes,

    [Required(ErrorMessage = "Submission Date is required")]
    DateOnly SubmittedDate,

    [Required]
    [AllowedValues(SubmissionStatus.Submitted, SubmissionStatus.Resubmitted , ErrorMessage = "Status must be Submitted or Resubmitted")]
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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagement.Api.TrackTaskModel;

namespace TraineeManagement.Api.SubmissionModel;

public class Submission
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id {get; set;}

    [Required(ErrorMessage = "Task Assignment Reference is Required")]
    public long TaskAssignmentId {get; set;}

// Navigation Property
    public TrackTask TrackTask {get; set;} = null!;

    [Required(ErrorMessage = "Submission Url is Required")]
    public string SubmissionUrl {get; set;} = string.Empty;

    [Required(ErrorMessage = "Notes are Required")]
    public string Notes {get; set;} = string.Empty;

    [Required(ErrorMessage = "Submission Date is required")]
    public DateOnly SubmittedDate {get; set;} = DateOnly.FromDateTime(DateTime.UtcNow);

    [Required]
    [AllowedValues(SubmissionStatus.Submitted, SubmissionStatus.Resubmitted , ErrorMessage = "Status must be Submitted or Resubmitted")]
    public SubmissionStatus Status {get; set;}

}

public enum SubmissionStatus
{
    Submitted,
    Resubmitted
}
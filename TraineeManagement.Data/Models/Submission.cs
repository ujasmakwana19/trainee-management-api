using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagement.Api.TrackTaskModel;
using TraineeManagement.Api.ValidationConstantUtils;

namespace TraineeManagement.Api.SubmissionModel;

public class Submission
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id {get; set;}

    [RequiredField]
    public long TaskAssignmentId {get; set;}

// Navigation Property
    public TrackTask TrackTask {get; set;} = null!;

    [RequiredField]
    [StringLengthField(ValidationConstant.MAX_LENTH_GENERIC_INPUT , 
    MinimumLength = ValidationConstant.MIN_LENTH_GENERIC_INPUT)]
    public string SubmissionUrl {get; set;} = string.Empty;

    [RequiredField]
    [StringLengthField(ValidationConstant.MAX_LENTH_GENERIC_INPUT , 
    MinimumLength = ValidationConstant.MIN_LENTH_GENERIC_INPUT)]
    public string Notes {get; set;} = string.Empty;

    [RequiredField]
    public DateOnly SubmittedDate {get; set;} = DateOnly.FromDateTime(DateTime.UtcNow);

    [RequiredField]
    [EnumDataTypeField(typeof(SubmissionStatus))]
    public SubmissionStatus Status {get; set;}

}

public enum SubmissionStatus
{
    Submitted,
    Resubmitted
}
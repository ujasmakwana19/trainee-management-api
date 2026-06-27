using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagement.Data.MentorModel;
using TraineeManagement.Data.SubmissionModel;
using TraineeManagement.Data.ValidationConstantUtils;

namespace TraineeManagement.Data.ReviewModel;

public class Review
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id {get; set;}

    [RequiredField]
    
    public long SubmissionId {get; set;}
    public Submission Submission {get; set;} = null!;

    [RequiredField]
    public long MentorId {get; set;} 
    public Mentor Mentor {get; set;} = null!;

    [RequiredField]
    [StringLengthField(ValidationConstant.MAX_LENTH_GENERIC_INPUT, 
    MinimumLength = ValidationConstant.MIN_LENTH_GENERIC_INPUT)]
    public string Feedback {get; set;} = string.Empty;

    public long Score {get; set;} 

    [RequiredField]
    [EnumDataTypeField(typeof(ReviewStatusValue))]
    public ReviewStatusValue ReviewStatus {get; set;}

    [RequiredField]
    public DateOnly ReviewedDate {get; set;} = DateOnly.FromDateTime(DateTime.UtcNow);
    
}

public enum ReviewStatusValue
{
    Accepted,
    ChangesRequired,
    Rejected
} 

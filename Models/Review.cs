using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagement.Api.MentorModel;
using TraineeManagement.Api.SubmissionModel;

namespace TraineeManagement.Api.ReviewModel;

public class Review
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id {get; set;}

    [Required(ErrorMessage = "Submission is required")]
    public long SubmissionId {get; set;}
    public Submission Submission {get; set;} = null!;

    [Required(ErrorMessage = "Reviewing Mentor is required")]
    public long MentorId {get; set;} 
    public Mentor Mentor {get; set;} = null!;

    [Required(ErrorMessage = "Mentor Feedback is required")]
    public string Feedback {get; set;} = string.Empty;

    public long Score {get; set;} 

    [Required]
    [AllowedValues(ReviewStatusValue.Accepted, ReviewStatusValue.ChangesRequired, ReviewStatusValue.Rejected, ErrorMessage = "Status must be Accepted, ChangesRequired or Rejected")]
    public ReviewStatusValue ReviewStatus {get; set;}

    [Required(ErrorMessage = "Review Date is required")]
    public DateOnly ReviewedDate {get; set;} = DateOnly.FromDateTime(DateTime.UtcNow);
    
}

public enum ReviewStatusValue
{
    Accepted,
    ChangesRequired,
    Rejected
} 

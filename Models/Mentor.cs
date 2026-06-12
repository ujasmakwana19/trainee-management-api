using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagement.Api.IDateTimeAutoService;
namespace TraineeManagement.Api.MentorModel;

public class Mentor : IDateTimeAuto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id {get; set;}

    [Required(ErrorMessage = "First Name is required")]
    [StringLength(50, ErrorMessage = "Must be less than or equals to 50 character")]
    public string FirstName { get; set; } = string.Empty;
        
    
    [Required(ErrorMessage = "Last Name is required")]
    [StringLength(50, ErrorMessage = "Must be less than or equals to 50 character")]
    public string LastName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Technical Expertise Details is Required")]
    public string Expertise {get; set;} = string.Empty;

    [Required]
    [AllowedValues(MentorStatus.Active, MentorStatus.Inactive, ErrorMessage = "Status must be Active, or Inactive")]
    public MentorStatus Status{get; set;} 

}

public enum MentorStatus
{
    Active,
    Inactive
}
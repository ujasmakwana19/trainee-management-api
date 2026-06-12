using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.MentorModel;
namespace TraineeManagement.Api.MentorDTO;

public record MentorRequestBody
(
    [Required(ErrorMessage = "First Name is required")]
    [StringLength(50,ErrorMessage ="Must be less than or equals to 50 character")]
    String FirstName,

    [Required(ErrorMessage = "Last Name is required")]
    [StringLength(50,ErrorMessage ="Must be less than or equals to 50 character")]
    String LastName,

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    String Email,

    [Required(ErrorMessage = "Technical Expertise Details is Required")]
    String Expertise,

    [Required]
    [AllowedValues(MentorStatus.Active, MentorStatus.Inactive, ErrorMessage = "Status must be Active, or Inactive")]
    MentorStatus Status
);

public record MentorResponse(
    long Id,
    String FirstName,
    String LastName,
    String Email,
    String Expertise,
    MentorStatus Status
);
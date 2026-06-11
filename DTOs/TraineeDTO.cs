using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.TraineeModel;
namespace TraineeManagement.Api.TraineeDTO;

public record CreateTraineeRequest
(
    [Required(ErrorMessage = "First Name is required")]
    [StringLength(50,ErrorMessage ="Must be less than or equals to 50 character")]
    String? FirstName,

    [Required(ErrorMessage = "Last Name is required")]
    [StringLength(50,ErrorMessage ="Must be less than or equals to 50 character")]
    String? LastName,

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    String? Email,

    [Required]
    String? TechStack,

    [Required]
    [AllowedValues(StatusValue.Active,StatusValue.Inactive,StatusValue.Completed, ErrorMessage = "Selected among the following Active , Inactive or Completed")]
    StatusValue? Status
);

public record UpdateTraineeRequest
(
    [Required(ErrorMessage = "First Name is required")]
    [StringLength(50,ErrorMessage ="Must be less than or equals to 50 character")]
    String? FirstName,

    [Required(ErrorMessage = "Last Name is required")]
    [StringLength(50,ErrorMessage ="Must be less than or equals to 50 character")]
    String? LastName,

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    String? Email,

    [Required]
    String? TechStack,

    [Required]
    [AllowedValues(StatusValue.Active,StatusValue.Inactive,StatusValue.Completed, ErrorMessage = "Selected among the following Active , Inactive or Completed")]
    StatusValue? Status
);


public record TraineeResponse
(
    long Id,
    String? FirstName,
    String? LastName,
    String? Email,
    String? TechStack,
    StatusValue? Status
);

public record TraineeInfoPagination(
    int? pageNumber,
    int? pageSize,
    int? totalRecords,
    List<TraineeResponse>? data
);
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagement.Api.IDateTimeAutoService;
namespace TraineeManagement.Api.TraineeModel
{
    public class Trainee : IDateTimeAuto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, ErrorMessage = "Must be less than or equals to 50 character")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, ErrorMessage = "Must be less than or equals to 50 character")]
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string TechStack { get; set; } = string.Empty;
        [Required]
        [AllowedValues(StatusValue.Active, StatusValue.Inactive, StatusValue.Completed, ErrorMessage = "Status must be Active, Inactive or Completed")]
        public StatusValue? Status { get; set; }
    }

    public enum StatusValue
    {
        Active,
        Inactive,
        Completed
    }

}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagement.Api.IDateTimeAutoService;
using TraineeManagement.Api.ValidationConstantUtils;
namespace TraineeManagement.Api.UserModel
{
    public class User : IDateTimeAuto
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public long Id { get; set; }

        [Required]
        [StringLength(ValidationConstant.MAX_LENTH_GENERIC_INPUT , 
        MinimumLength = ValidationConstant.MIN_LENTH_GENERIC_INPUT)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(ValidationConstant.PASSWORD_INPUT)] // Large length to safely store long cryptographic hashes
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [EnumDataType(typeof(UserRole))]
        public UserRole Role { get; set; }

    }

    public enum UserRole
    {
        Admin,
        Mentor,
        Trainee
    }
}


    
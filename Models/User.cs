using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TraineeManagement.Api.UserModel
{
    public class User
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public long Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)] // Large length to safely store long cryptographic hashes
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [AllowedValues(UserRole.Admin, UserRole.Mentor, UserRole.Trainee, ErrorMessage = "Role must be Admin, Mentor, or Trainee.")]
        public UserRole Role { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    }

    public enum UserRole
    {
        Admin,
        Mentor,
        Trainee
    }
}


    
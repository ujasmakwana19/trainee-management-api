using System.ComponentModel.DataAnnotations;
using TraineeManagement.Data.UserModel;
namespace TraineeManagement.Data.UserDTO;

public record LoginUserRequest
(
    [Required]
    [EmailAddress]
    String Email,
    [RequiredField]
    String Password
);

public record LoginUserResponse
(
    String Token,
    long ExpiriesIn,
    UserRecord User
);

public record UserRecord(
    long Id,
    String Username,
    String Email,
    UserRole Role
);

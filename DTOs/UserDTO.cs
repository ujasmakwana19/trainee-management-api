using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.UserModel;
namespace TraineeManagement.Api.UserDTO;

public record LoginUserRequest
(
    [Required(ErrorMessage = "User Name is required")]
    String Username,
    [Required(ErrorMessage = "Password is required")]
    String Password
);

public record LoginUserResponse
(
    String? Token,
    long? ExpiriesIn,
    UserRecord User
);

public record UserRecord(
    long? Id,
    String? Username,
    UserRole? Role
);

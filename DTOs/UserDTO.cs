using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.UserModel;
namespace TraineeManagement.Api.UserDTO;

public record LoginUserRequest
(
    [Required]
    String Username,
    [Required]
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
    UserRole Role
);

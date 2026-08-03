using TraineeManagement.Data.UserDTO;

namespace TraineeManagement.Api.UserServices;

public interface IUserService
{
    Task<LoginResult> Login(LoginUserRequest request);
    Task<LoginUserResponse> GetToken(string token);
    Task<UserProfileResponse> getUserProfile();
}
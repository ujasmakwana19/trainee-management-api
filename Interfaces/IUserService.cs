using TraineeManagement.Api.UserDTO;

namespace TraineeManagement.Api.UserServices;

public interface IUserService
{
    Task<LoginUserResponse> Login(LoginUserRequest request);
}
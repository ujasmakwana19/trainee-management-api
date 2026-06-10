using TraineeManagement.Api.UserModel;
using TraineeManagement.Api.TraineeDTO;
using TraineeManagement.Api.UserDTO;

namespace TraineeManagement.Api.UserServices;

public interface IUserService
{
    public Task<LoginUserResponse?> Login(LoginUserRequest userInfo);
}
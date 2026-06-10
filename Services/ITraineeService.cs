using TraineeManagement.Api.TraineeModel;
using TraineeManagement.Api.TraineeDTO;

namespace TraineeManagement.Api.TraineeServices;

public interface ITraineeService
{
    Task<Trainee?> FetchTrainee(long id);
    Task<bool> DeleteTraineeService(long id);
    Task<IEnumerable<TraineeResponse>?> GetAllTraineesService();
    Task<TraineeResponse?> GetTraineeResponseByIdService(long id);
    Task<TraineeResponse?> CreateTraineeService(CreateTraineeRequest trainee);
    Task<TraineeResponse?> UpdateTraineeService(long id, UpdateTraineeRequest trainee);
    Task<IEnumerable<TraineeResponse>?> SearchTraineeService(String s);
}
using TraineeManagement.Data.TraineeModel;
using TraineeManagement.Data.TraineeDTO;

namespace TraineeManagement.Api.TraineeServices;

public interface ITraineeService
{
    Task<Trainee> FetchTrainee(long id);
    Task DeleteTraineeService(long id);
    Task<IEnumerable<TraineeResponse>> GetAllTraineesService();
    Task<TraineeResponse> GetTraineeResponseByIdService(long id, CancellationToken cancellationToken);
    Task<TraineeResponse> CreateTraineeService(CreateTraineeRequest trainee);
    Task<TraineeResponse> UpdateTraineeService(long id, UpdateTraineeRequest trainee);
    Task<IEnumerable<TraineeResponse>> SearchTraineeService(String s);
    Task<TraineeInfoPagination> SearchTraineePaginationService(int pageNumber, int pageSize, String search, StatusValue status);
}
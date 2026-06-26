using TraineeManagement.Data.TraineeDTO;

namespace TrainingDirectory.TraineeInterface;

public interface ITraineeService
{
    Task<TraineeResponse> GetById(long id);
}
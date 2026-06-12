using TraineeManagement.Api.TaskDTO;

namespace TraineeManagement.Api.LearningTaskServices;

public interface ILearningTaskService
{
    Task DeleteTask(long id);
    Task<IEnumerable<TaskResponseData>> GetAll();
    Task<TaskResponseData> GetById(long id);
    Task<TaskResponseData> CreateTask(TaskRequestBody mentor);
    Task<TaskResponseData> UpdateTask(long Id, TaskRequestBody mentor);
}



    
    

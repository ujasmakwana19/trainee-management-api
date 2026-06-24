using TraineeManagement.Api.TrackTaskDTO;

namespace TraineeManagement.Api.TrackTaskService;

public interface ITrackTaskService
{
    Task<TrackTaskResponse> CreateTrackTaskAsync(TrackTaskRequestBody trackTaskRequestBody);
    Task<TrackTaskPopulatedResponseBody> GetTrackTaskByIdAsync(long id);
    Task<IEnumerable<TrackTaskResponse>> GetAllTasks();
    Task<TrackTaskResponse> UpdateTrackTaskAsync(long id, TrackTaskUpdateRequestBody body);
}
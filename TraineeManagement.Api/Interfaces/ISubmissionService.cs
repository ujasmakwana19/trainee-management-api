using TraineeManagement.Api.SubmissionDTO;

namespace TraineeManagement.Api.SubmissionService;

public interface ISubmissionService
{
    Task<SubmissionResponse> CreateSubmission(SubmissionRequestBody body);
    Task<SubmissionResponse> GetSubmissionById(long id);
    Task<IEnumerable<SubmissionResponse>> GetAll();
}
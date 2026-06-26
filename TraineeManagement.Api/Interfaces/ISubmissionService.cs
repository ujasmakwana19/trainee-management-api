using TraineeManagement.Data.SubmissionDTO;

namespace TraineeManagement.Api.SubmissionService;

public interface ISubmissionService
{
    Task<SubmissionResponse> CreateSubmission(SubmissionRequestBody body);
    Task<SubmissionResponse> GetSubmissionById(long id);
    Task<IEnumerable<SubmissionResponse>> GetAll();
}
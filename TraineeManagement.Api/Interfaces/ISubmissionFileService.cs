using TraineeManagement.Api.FileServices;
using TraineeManagement.Data.SubmissionFileModel;

namespace TraineeManagement.Api.SubmissionFileService;

public interface ISubmissionFileService
{
    Task<bool> IsSubmissionExists(long submissionId);
    Task<long> SaveMetadataAsync(long submissionId, long uploadedByUserId, SavedFileResult savedFile, 
    CancellationToken cancellationToken);
    Task<SubmissionFile> GetByIdAsync(long fileId);
    Task DeleteMetadataAsync(long fileId, CancellationToken cancellationToken);
}
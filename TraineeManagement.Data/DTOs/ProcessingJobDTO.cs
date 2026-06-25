using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagement.Data.ProcessingJobModel;
namespace TraineeManagement.Data.ProcessingJobDTO;

public record ProcessingJobDto
(
    [RequiredField]
    Guid MessageId,

    [RequiredField]
    Guid CoorelationId,

    [RequiredField]
    long SubmissionId,

    [EnumDataTypeField(typeof(ProcessingJobStatus))]
    
    ProcessingJobStatus Status = ProcessingJobStatus.Queued,
    
    int Attempts = 0,

    string? ErrorSummary = null,
    
    DateTime? StartedAt = null,
    
    DateTime? CompletedAt = null
);


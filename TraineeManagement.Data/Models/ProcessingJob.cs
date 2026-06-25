using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagement.Api.SubmissionFileModel;

namespace TraineeManagement.Data.ProcessingJobModel;

public class ProcessingJob
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id {get; set;}

    [RequiredField]
    public Guid MessageId {get; set;}

    [RequiredField]
    public Guid CoorelationId {get; set;}

    [RequiredField]
    public long SubmissionId {get; set;}

    [EnumDataTypeField(typeof(ProcessingJobStatus))]
    public ProcessingJobStatus Status {get; set;} = ProcessingJobStatus.Queued;
    public int Attempts {get; set;} = 0;

    public string? ErrorSummary {get; set;} 
    public DateTime? StartedAt {get; set;} 
    public DateTime? CompletedAt {get; set;}
}

public enum ProcessingJobStatus
{
    Queued,
    Processing,
    Completed,
    Failed
}
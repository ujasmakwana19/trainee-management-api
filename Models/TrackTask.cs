using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagement.Api.TraineeModel;
using TraineeManagement.Api.MentorModel;
using TraineeManagement.Api.TaskModel;
using TraineeManagement.Api.ValidationConstantUtils;

namespace TraineeManagement.Api.TrackTaskModel;
public class TrackTask
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id {get; set;}

    [RequiredField]
    public long TraineeId { get; set; } 
    
    [RequiredField]
    public long MentorId { get; set; } 

    [RequiredField]
    public long LearningTaskId { get; set; } 
    
    // Navigation Properties
    public Trainee Trainee { get; set; } = null!;
    public Mentor Mentor { get; set; } = null!;
    public LearningTask LearningTask { get; set; } = null!;

    [RequiredField]
    public DateOnly AssignedDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    
    [RequiredField]
    public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    
    [RequiredField]
    [EnumDataTypeField(typeof(TaskAssignmentValue))]
    public TaskAssignmentValue Status {get; set;}
    public string Remark { get; set; } = string.Empty;

}

public enum TaskAssignmentValue
{
    Assigned,
    Inprogess,
    Submitted,
    Reviewed,
    Completed
}

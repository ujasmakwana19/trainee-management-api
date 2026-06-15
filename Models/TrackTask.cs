using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagement.Api.TraineeModel;
using TraineeManagement.Api.MentorModel;
using TraineeManagement.Api.TaskModel;

namespace TraineeManagement.Api.TrackTaskModel;
public class TrackTask
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id {get; set;}

    [Required(ErrorMessage = "TraineeId is required")]
    public long TraineeId { get; set; } 
    
    [Required(ErrorMessage = "MentorId is required")]
    public long MentorId { get; set; } 

    [Required(ErrorMessage = "TaskId is required")]
    public long LearningTaskId { get; set; } 
    
    // Navigation Properties
    public Trainee Trainee { get; set; } = null!;
    public Mentor Mentor { get; set; } = null!;
    public LearningTask LearningTask { get; set; } = null!;

    [Required(ErrorMessage = "Assigned Date is required")]
    public DateOnly AssignedDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    
    [Required(ErrorMessage = "Due Date is required")]
    public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    
    [Required]
    [AllowedValues(TaskAssignmentValue.Assigned, TaskAssignmentValue.Inprogess, TaskAssignmentValue.Submitted, TaskAssignmentValue.Reviewed, TaskAssignmentValue.Completed , ErrorMessage = "Status must be Assigned, Inprogess, Submitted, Reviewed or Completed")]
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

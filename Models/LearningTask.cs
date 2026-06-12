using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagement.Api.IDateTimeAutoService;

namespace TraineeManagement.Api.TaskModel;
public class LearningTask : IDateTimeAuto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id {get; set;}

    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Tech Stack is required")]
    public string ExpectedTechStack { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Due Date is required")]
    public DateTime DueDate { get; set; } = DateTime.UtcNow;

    [Required]
    [AllowedValues(TaskStatusValue.Draft,TaskStatusValue.Published,TaskStatusValue.Closed , ErrorMessage = "Status must be Draft, Published or Closed")]
    public TaskStatusValue Status {get; set;}
}

public enum TaskStatusValue
{
    Draft,
    Published,
    Closed
}

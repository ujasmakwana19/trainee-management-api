using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagement.Api.IDateTimeAutoService;
using TraineeManagement.Api.TraineeModel;

namespace TraineeManagement.Api.TrackTaskModel;
public class TrackTask
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id {get; set;}

    [Required(ErrorMessage = "TraineeId is required")]
    public Trainee TraineeId { get; set; } = new Trainee();
    
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

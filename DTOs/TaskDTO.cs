using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.TaskModel;

namespace TraineeManagement.Api.TaskDTO;

public record TaskRequestBody
(
    [Required(ErrorMessage = "Title is required")]
    String Title,

    [Required(ErrorMessage = "Description is required")]
    String Description,

    [Required(ErrorMessage = "Tech Stack is required")]
    String ExpectedTechStack,

    [Required(ErrorMessage = "Due Date is required")]
    DateTime DueDate,

    [Required]
    [AllowedValues(TaskStatusValue.Draft,TaskStatusValue.Published,TaskStatusValue.Closed , ErrorMessage = "Status must be Draft, Published or Closed")]
    TaskStatusValue Status
);

public record TaskResponseData(
    long Id,
    String Title,
    String Description,
    String ExpectedTechStack,
    DateTime DueDate,
    TaskStatusValue Status
);
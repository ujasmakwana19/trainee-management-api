using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.MentorDTO;
using TraineeManagement.Api.TaskDTO;
using TraineeManagement.Api.TrackTaskModel;
using TraineeManagement.Api.TraineeDTO;
namespace TraineeManagement.Api.TrackTaskDTO;

public record TrackTaskRequestBody
(
    [Required(ErrorMessage = "TraineeId is required")]
    long TraineeId,

    [Required(ErrorMessage = "MentorId is required")]
    long MentorId,
    
    [Required(ErrorMessage = "LearningTaskId is required")]
    long LearningTaskId,
    
    [Required(ErrorMessage = "AssignedDate is required")]
    DateOnly AssignedDate,
    
    [Required(ErrorMessage = "DueDate is required")]
    DateOnly DueDate,
    
    [Required]
    [AllowedValues(TaskAssignmentValue.Assigned, TaskAssignmentValue.Inprogess, TaskAssignmentValue.Submitted, TaskAssignmentValue.Reviewed, TaskAssignmentValue.Completed , ErrorMessage = "Status must be Assigned, Inprogess, Submitted, Reviewed or Completed")]
    TaskAssignmentValue Status,
    
    string Remark
);

public record TrackTaskUpdateRequestBody
(
    [Required]
    [AllowedValues(TaskAssignmentValue.Assigned, TaskAssignmentValue.Inprogess, TaskAssignmentValue.Submitted, TaskAssignmentValue.Reviewed, TaskAssignmentValue.Completed , ErrorMessage = "Status must be Assigned, Inprogess, Submitted, Reviewed or Completed")]
    TaskAssignmentValue Status
);

public record TrackTaskResponse
(
    long Id,
    long TraineeId,
    long MentorId,
    long LearningTaskId,
    DateOnly AssignedDate,
    DateOnly DueDate,
    TaskAssignmentValue Status,
    string Remark
);

public record TrackTaskPopulatedResponseBody
(
    long Id,
    TraineeResponse Trainee,
    MentorResponse Mentor,
    TaskResponseData LearningTask,
    DateOnly AssignedDate,
    DateOnly DueDate,
    TaskAssignmentValue Status,
    string Remark
);

public record TrackTaskNameResponseBody
(
    long Id,
    string TraineeName,
    string MentorName,
    string LearningTaskTitle,
    DateOnly AssignedDate,
    DateOnly DueDate,
    TaskAssignmentValue Status,
    string Remark
);
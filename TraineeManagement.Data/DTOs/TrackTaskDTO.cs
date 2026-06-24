using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.MentorDTO;
using TraineeManagement.Api.TaskDTO;
using TraineeManagement.Api.TrackTaskModel;
using TraineeManagement.Api.TraineeDTO;
namespace TraineeManagement.Api.TrackTaskDTO;

public record TrackTaskRequestBody
(
    [RequiredField]
    long TraineeId,

    [RequiredField]
    long MentorId,
    
    [RequiredField]
    long LearningTaskId,
    
    [RequiredField]
    DateOnly AssignedDate,
    
    [RequiredField]
    DateOnly DueDate,
    
    [RequiredField]
    [EnumDataTypeField(typeof(TaskAssignmentValue))]
    TaskAssignmentValue Status,
    
    string Remark
);

public record TrackTaskUpdateRequestBody
(
    [RequiredField]
    [EnumDataTypeField(typeof(TaskAssignmentValue))]
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
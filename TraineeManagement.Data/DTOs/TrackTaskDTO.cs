using System.ComponentModel.DataAnnotations;
using TraineeManagement.Data.MentorDTO;
using TraineeManagement.Data.TaskDTO;
using TraineeManagement.Data.TrackTaskModel;
using TraineeManagement.Data.TraineeDTO;
namespace TraineeManagement.Data.TrackTaskDTO;

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

public record TrackTaskPersonalResponse(
    long Id,
    long TraineeId,
    string TraineeName,
    long MentorId,
    string MentorName,
    long LearningTaskId,
    string TaskTitle,
    DateOnly AssignedDate,
    DateOnly DueDate,
    TaskAssignmentValue Status,
    string Remark
);
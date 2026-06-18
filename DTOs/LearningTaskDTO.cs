using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.TaskModel;
using TraineeManagement.Api.ValidationConstantUtils;

namespace TraineeManagement.Api.TaskDTO;

public record TaskRequestBody
(
    [RequiredField]
    [StringLengthField(ValidationConstant.MAX_LENTH_GENERIC_INPUT, 
    MinimumLength = ValidationConstant.MIN_LENTH_GENERIC_INPUT)] 
    String Title,

    [RequiredField]
    [StringLengthField(ValidationConstant.MAX_LENTH_LARGE_INPUT, 
    MinimumLength = ValidationConstant.MIN_LENTH_GENERIC_INPUT)] 
    String Description,

    [RequiredField]
    [StringLengthField(ValidationConstant.MAX_LENTH_GENERIC_INPUT, 
    MinimumLength = ValidationConstant.MIN_LENTH_GENERIC_INPUT)]  
    String ExpectedTechStack,

    [RequiredField]
    DateTime DueDate,

    [RequiredField]
    [EnumDataTypeField(typeof(TaskStatusValue))]
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
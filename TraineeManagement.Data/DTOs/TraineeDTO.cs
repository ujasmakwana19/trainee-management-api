using System.ComponentModel.DataAnnotations;
using TraineeManagement.Data.TraineeModel;
using TraineeManagement.Data.ValidationConstantUtils;
namespace TraineeManagement.Data.TraineeDTO;

public record CreateTraineeRequest
(
    [RequiredField]
    [StringLengthField(ValidationConstant.MAX_LENTH_NAME_INPUT, ValidationConstant.MIN_LENTH_GENERIC_INPUT)]
    String FirstName,

    [RequiredField]
    [StringLengthField(ValidationConstant.MAX_LENTH_NAME_INPUT, ValidationConstant.MIN_LENTH_GENERIC_INPUT)]
    String LastName,

    [RequiredField]
    [EmailField]
    String Email,

    [RequiredField]
    String TechStack,
    
    [RequiredField]
    String Password,

    [RequiredField]
    [EnumDataTypeField(typeof(StatusValue))]
    StatusValue Status
);
public record UpdateTraineeRequest
(
    [RequiredField]
    [StringLengthField(ValidationConstant.MAX_LENTH_NAME_INPUT, ValidationConstant.MIN_LENTH_GENERIC_INPUT)]
    String FirstName,

    [RequiredField]
    [StringLengthField(ValidationConstant.MAX_LENTH_NAME_INPUT, ValidationConstant.MIN_LENTH_GENERIC_INPUT)]
    String LastName,

    [RequiredField]
    [EmailField]
    String Email,

    [RequiredField]
    String TechStack,

    [RequiredField]
    [EnumDataTypeField(typeof(StatusValue))]
    StatusValue Status
);

public record TraineeResponse
(
    long Id,
    String FirstName,
    String LastName,
    String Email,
    String TechStack,
    StatusValue Status
);

public record TraineeInfoPagination(
    int pageNumber,
    int pageSize,
    int totalRecords,
    List<TraineeResponse> data
);
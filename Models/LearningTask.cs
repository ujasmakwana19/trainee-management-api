using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagement.Api.IDateTimeAutoService;
using TraineeManagement.Api.ValidationConstantUtils;

namespace TraineeManagement.Api.TaskModel;
public class LearningTask : IDateTimeAuto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id {get; set;}

    [RequiredField]
    [StringLengthField(ValidationConstant.MAX_LENTH_GENERIC_INPUT, 
    MinimumLength = ValidationConstant.MIN_LENTH_GENERIC_INPUT)]    
    public string Title { get; set; } = string.Empty;
    
    [RequiredField]
    [StringLengthField(ValidationConstant.MAX_LENTH_LARGE_INPUT, 
    MinimumLength = ValidationConstant.MIN_LENTH_GENERIC_INPUT)]    
    public string Description { get; set; } = string.Empty;
    
    [RequiredField]
    [StringLengthField(ValidationConstant.MAX_LENTH_GENERIC_INPUT, 
    MinimumLength = ValidationConstant.MIN_LENTH_GENERIC_INPUT)]    
    public string ExpectedTechStack { get; set; } = string.Empty;
    
    
    [RequiredField]
    public DateTime DueDate { get; set; } = DateTime.UtcNow;

    [RequiredField]
    [EnumDataTypeField(typeof(TaskStatusValue))]
    public TaskStatusValue Status {get; set;}
}

public enum TaskStatusValue
{
    Draft,
    Published,
    Closed
}

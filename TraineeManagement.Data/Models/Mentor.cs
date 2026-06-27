using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagement.Data.IDateTimeAutoService;
using TraineeManagement.Data.ValidationConstantUtils;
namespace TraineeManagement.Data.MentorModel;

public class Mentor : IDateTimeAuto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id {get; set;}

    [RequiredField]
    [StringLengthField(ValidationConstant.MAX_LENTH_NAME_INPUT, 
    MinimumLength = ValidationConstant.MIN_LENTH_GENERIC_INPUT)]
    public string FirstName { get; set; } = string.Empty;
        
    
    [RequiredField]
    [StringLengthField(ValidationConstant.MAX_LENTH_NAME_INPUT,
    MinimumLength = ValidationConstant.MIN_LENTH_GENERIC_INPUT)]
    public string LastName { get; set; } = string.Empty;
    
    [RequiredField]
    [EmailField]
    public string Email { get; set; } = string.Empty;

    [RequiredField]
    [StringLengthField(ValidationConstant.MAX_LENTH_GENERIC_INPUT,
    MinimumLength = ValidationConstant.MIN_LENTH_GENERIC_INPUT)]
    public string Expertise {get; set;} = string.Empty;

    [Required]
    [EnumDataTypeField(typeof(MentorStatus))]
    public MentorStatus Status{get; set;} 

}

public enum MentorStatus
{
    Active,
    Inactive
}
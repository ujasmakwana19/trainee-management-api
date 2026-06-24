using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagement.Api.IDateTimeAutoService;
using TraineeManagement.Api.ValidationConstantUtils;
namespace TraineeManagement.Api.TraineeModel
{
    public class Trainee : IDateTimeAuto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [RequiredField]
        [StringLengthField(ValidationConstant.MAX_LENTH_NAME_INPUT, ValidationConstant.MIN_LENTH_GENERIC_INPUT)]
        public string FirstName { get; set; } = string.Empty;
        
        [RequiredField]
        [StringLengthField(ValidationConstant.MAX_LENTH_NAME_INPUT, ValidationConstant.MIN_LENTH_GENERIC_INPUT)]
        public string LastName { get; set; } = string.Empty;

        [RequiredField]
        [EmailField]
        public string Email { get; set; } = string.Empty;

        [RequiredField]
        public string TechStack { get; set; } = string.Empty;

        [RequiredField]
        [EnumDataTypeField(typeof(StatusValue))]
        public StatusValue Status { get; set; }
    }

    public enum StatusValue
    {
        Active,
        Inactive,
        Completed
    }

}
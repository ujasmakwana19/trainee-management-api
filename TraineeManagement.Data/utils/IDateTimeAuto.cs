// Models/BaseEntity.cs
namespace TraineeManagement.Api.IDateTimeAutoService;

public abstract class IDateTimeAuto
{
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
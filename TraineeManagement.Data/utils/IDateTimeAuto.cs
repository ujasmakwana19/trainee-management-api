// Models/BaseEntity.cs
namespace TraineeManagement.Data.IDateTimeAutoService;

public abstract class IDateTimeAuto
{
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
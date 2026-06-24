using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TraineeManagement.Api.CacheServices;

public class CacheKey
{
    private const string app = "tms:";   
    public const string traineeId = $"{app}trainee:";
}
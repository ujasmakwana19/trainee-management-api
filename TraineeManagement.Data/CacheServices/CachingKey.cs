using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TraineeManagement.Data.CacheServices;

public class CacheKey
{
    private const string app = "tms:";   
    public const string traineeId = $"{app}trainee:";
    public const string traineeall = $"{app}trainee:all";
    public const string mentorId = $"{app}mentor:";
    public const string mentorall = $"{app}mentor:all";
    public const string taskId = $"{app}task:";
    public const string trackTaskId = $"{app}taskAssignment:";
    public const string trackTaskAll = $"{app}taskAssignment:all";
    public const string submissionId = $"{app}submission:";
    public const string reviewAll = $"{app}review:all";

}

public class CacheTTL
{
    public const int GETS_TTL_MIN = 15;   
}
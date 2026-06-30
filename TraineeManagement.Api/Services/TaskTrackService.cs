using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data.DataBaseContext;
using TraineeManagement.Data.TrackTaskDTO;
using TraineeManagement.Data.TraineeDTO;
using TraineeManagement.Data.MentorDTO;
using TraineeManagement.Data.TaskDTO;
using TraineeManagement.Data.TrackTaskModel;
using TraineeManagement.WebCommons.ExceptionUtils;
using TraineeManagement.WebCommons.ErrorCodesUtils;
using TraineeManagement.Data.CacheServices;
namespace TraineeManagement.Api.TrackTaskService;

public class TrackTaskService : ITrackTaskService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TrackTaskService>_logger;
    private readonly ICacheService _cache;

    public TrackTaskService(AppDbContext context, ILogger<TrackTaskService> logger, ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    private TrackTaskResponse ToResponse(TrackTask trackTask)
    {
        return new TrackTaskResponse
        (
            Id: trackTask.Id,
            TraineeId: trackTask.TraineeId,
            MentorId: trackTask.MentorId,
            LearningTaskId: trackTask.LearningTaskId,
            AssignedDate: trackTask.AssignedDate,
            DueDate: trackTask.DueDate,
            Status: trackTask.Status,
            Remark: trackTask.Remark
        );
    }

    private async Task<TrackTask> FetchTrackTask(long id){
        TrackTask? t = await _context.TrackTasks.FirstOrDefaultAsync(t => t.Id == id); 
        if(t is null)
        {
            throw new NotFoundException(ErrorCodes.NOT_FOUND_TASK_ASSIGNMENT);
        }
        return t;
    }

    public async Task<TrackTaskResponse> CreateTrackTaskAsync(TrackTaskRequestBody body)
    {
        if(body.DueDate < body.AssignedDate)
        {
            throw new BadRequestException(ErrorCodes.NOT_FOUND_TASK_ASSIGNMENT);
        }

        TrackTask trackTask = new TrackTask
        {
            TraineeId = body.TraineeId,
            MentorId = body.MentorId,
            LearningTaskId = body.LearningTaskId,
            AssignedDate = body.AssignedDate,
            DueDate = body.DueDate,
            Status = body.Status,
            Remark = body.Remark
        };

        _context.TrackTasks.Add(trackTask);
        await _context.SaveChangesAsync();
        _logger.LogInformation("TrackTask {TrackTaskId} created successfully", trackTask.Id);
        await _cache.RemoveAsync(CacheKey.trackTaskAll);
        return ToResponse(trackTask);
    }

    public async Task<IEnumerable<TrackTaskResponse>> GetAllTasks()
    {
        IEnumerable<TrackTaskResponse>? trackTasks = await _cache.GetAsync<IEnumerable<TrackTaskResponse>>(CacheKey.trackTaskAll);

        if(trackTasks is null)
        {    
            trackTasks = await _context.TrackTasks
                                                .Select(t => new TrackTaskResponse(
                                                    t.Id,
                                                    t.TraineeId,
                                                    t.MentorId,
                                                    t.LearningTaskId,
                                                    t.AssignedDate,
                                                    t.DueDate,
                                                    t.Status,
                                                    t.Remark
                                                ))
                                                .ToListAsync();
            if(trackTasks.Any())
                await _cache.SetAsync<IEnumerable<TrackTaskResponse>>(CacheKey.trackTaskAll,trackTasks,CacheTTL.GETS_TTL_MIN);
        }
        return trackTasks;
    }

    public async Task<TrackTaskPopulatedResponseBody> GetTrackTaskByIdAsync(long id)
    {
        string cacheKey = CacheKey.trackTaskId + $"{id}";
        TrackTaskPopulatedResponseBody? trackTask = await _cache.GetAsync<TrackTaskPopulatedResponseBody>(cacheKey);

        if(trackTask is null)
        {
            TrackTask? task = await _context.TrackTasks
                .Include(t => t.Trainee)
                .Include(t => t.Mentor)
                .Include(t => t.LearningTask)
                .FirstOrDefaultAsync(t => t.Id == id) ; 
            
            if(task is null)
            {
                throw new NotFoundException(ErrorCodes.NOT_FOUND_TASK_ASSIGNMENT);
            }

            trackTask = new TrackTaskPopulatedResponseBody
            (
                Id: task.Id,
                Trainee: new TraineeResponse(task.Trainee.Id, task.Trainee.FirstName, task.Trainee.LastName, task.Trainee.Email, task.Trainee.TechStack, task.Trainee.Status),
                Mentor: new MentorResponse(task.Mentor.Id, task.Mentor.FirstName,task.Mentor.LastName, task.Mentor.Email, task.Mentor.Expertise, task.Mentor.Status),
                LearningTask: new TaskResponseData(task.LearningTask.Id, task.LearningTask.Title, task.LearningTask.Description, task.LearningTask.ExpectedTechStack, task.LearningTask.DueDate, task.LearningTask.Status),
                AssignedDate: task.AssignedDate,
                DueDate: task.DueDate,
                Status: task.Status,
                Remark: task.Remark
            );

            await _cache.SetAsync<TrackTaskPopulatedResponseBody>(cacheKey,trackTask,CacheTTL.GETS_TTL_MIN);
        }
        

        return trackTask;
    }

    public async Task<TrackTaskResponse> UpdateTrackTaskAsync(long id, TrackTaskUpdateRequestBody body)
    {

        TrackTask trackTask = await FetchTrackTask(id);

        trackTask.Status = body.Status;

        await _context.SaveChangesAsync();
        _logger.LogInformation("TrackTask {TrackTaskId} updated successfully", trackTask.Id);
        await _cache.RemoveAsync(CacheKey.trackTaskAll);
        await _cache.RemoveAsync(CacheKey.trackTaskId + $"{id}");
        return ToResponse(trackTask);
    }
}
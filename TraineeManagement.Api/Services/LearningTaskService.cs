using Microsoft.EntityFrameworkCore;
using TraineeManagement.Contracts.CacheServices;
using TraineeManagement.Data.DataBaseContext;
using TraineeManagement.Contracts.ErrorCodesUtils;
using TraineeManagement.Contracts.ExceptionUtils;
using TraineeManagement.Data.TaskDTO;
using TraineeManagement.Data.TaskModel;

namespace TraineeManagement.Api.LearningTaskServices;

public class LearningTaskService : ILearningTaskService
{
    private readonly AppDbContext _context;
    private readonly ILogger<LearningTaskService> _logger;
    private readonly ICacheService _cache;

    public LearningTaskService(AppDbContext context, ILogger<LearningTaskService> logger, ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    private static TaskResponseData ToResponse(LearningTask task)
    {
        return new TaskResponseData(
            task.Id,
            task.Title,
            task.Description,
            task.ExpectedTechStack,
            task.DueDate,
            task.Status
        );
    }

    private async Task<LearningTask> FetchTask(long id)
    {
        LearningTask? task = await _context.LearningTasks.FirstOrDefaultAsync(t => t.Id == id);
        if (task is null)
        {
            throw new NotFoundException(ErrorCodes.NOT_FOUND_TASK);
        }
        return task;
    }

    // GETALL
    public async Task<IEnumerable<TaskResponseData>> GetAll()
    {
        IEnumerable<TaskResponseData> tasks = await _context.LearningTasks
                                        .Select(t => new TaskResponseData(
                                            t.Id,
                                            t.Title,
                                            t.Description,
                                            t.ExpectedTechStack,
                                            t.DueDate,
                                            t.Status
                                        ))
                                        .ToListAsync();

        return tasks;
    }

    // GET by ID
    public async Task<TaskResponseData> GetById(long id)
    {
        string cacheKey = CacheKey.taskId + $"{id}";
        TaskResponseData? task = await _cache.GetAsync<TaskResponseData>(cacheKey);

        if(task is null)
        {    
            task = await _context.LearningTasks
                                    .Where(t => t.Id == id)
                                    .Select(t => new TaskResponseData(
                                                t.Id,
                                                t.Title,
                                                t.Description,
                                                t.ExpectedTechStack,
                                                t.DueDate,
                                                t.Status
                                            ))
                                    .FirstOrDefaultAsync();
            if (task is null)
            {
                throw new NotFoundException(ErrorCodes.NOT_FOUND_TASK);
            }

            await _cache.SetAsync<TaskResponseData>(cacheKey, task, CacheTTL.GETS_TTL_MIN);
        }
        return task;
    }

    // CREATE
    public async Task<TaskResponseData> CreateTask(TaskRequestBody taskInfo)
    {

        LearningTask taskData = new LearningTask
        {
            Title = taskInfo.Title,
            Description = taskInfo.Description,
            ExpectedTechStack = taskInfo.ExpectedTechStack,
            DueDate = taskInfo.DueDate,
            Status = taskInfo.Status
        };

        _context.LearningTasks.Add(taskData);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Task {TaskId} created successfully", taskData.Id);
        return ToResponse(taskData);
    }

    // UPDATE
    public async Task<TaskResponseData> UpdateTask(long Id, TaskRequestBody taskInfo)
    {
        LearningTask taskData = await FetchTask(Id);

        taskData.Title = taskInfo.Title;
        taskData.Description = taskInfo.Description;
        taskData.ExpectedTechStack = taskInfo.ExpectedTechStack;
        taskData.DueDate = taskInfo.DueDate;
        taskData.Status = taskInfo.Status;


        _context.LearningTasks.Update(taskData);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Task {TaskId} updated successfully", taskData.Id);
        
        await _cache.RemoveAsync(CacheKey.taskId + $"{Id}");
        TaskResponseData task = ToResponse(taskData);
        await _cache.SetAsync<TaskResponseData>(CacheKey.taskId + $"{task.Id}", task, CacheTTL.GETS_TTL_MIN);
        
        return task;
    }

    // DELETE
    public async Task DeleteTask(long id)
    {
        LearningTask taskData = await FetchTask(id);

        _context.LearningTasks.Remove(taskData);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Task {TaskId} deleted successfully", taskData.Id);
        
        await _cache.RemoveAsync(CacheKey.taskId + $"{id}");
        return;
    }
}
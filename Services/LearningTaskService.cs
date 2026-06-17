using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.ExceptionUtils;
using TraineeManagement.Api.TaskDTO;
using TraineeManagement.Api.TaskModel;
namespace TraineeManagement.Api.LearningTaskServices;

public class LearningTaskService : ILearningTaskService
{
    private readonly AppDbContext _context;
    private readonly ILogger<LearningTaskService> _logger;

    public LearningTaskService(AppDbContext context, ILogger<LearningTaskService> logger)
    {
        _context = context;
        _logger = logger;
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
            throw new NotFoundException("Task Not Found");
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
        TaskResponseData? task = await _context.LearningTasks
                                .Where(t => t.Id == id)
                                .Select(t => new TaskResponseData(
                                            t.Id,
                                            t.Title,
                                            t.Description,
                                            t.ExpectedTechStack,
                                            t.DueDate,
                                            t.Status
                                        ))
                                .FirstOrDefaultAsync(t => t.Id == id);
        if (task is null)
        {
            throw new NotFoundException("Task Not Found");
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
        return ToResponse(taskData);
    }

    // DELETE
    public async Task DeleteTask(long id)
    {
        LearningTask taskData = await FetchTask(id);

        _context.LearningTasks.Remove(taskData);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Task {TaskId} deleted successfully", taskData.Id);
        return;
    }
}
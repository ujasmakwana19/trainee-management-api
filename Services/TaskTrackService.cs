using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.TrackTaskDTO;
using TraineeManagement.Api.TraineeDTO;
using TraineeManagement.Api.MentorDTO;
using TraineeManagement.Api.TaskDTO;
using TraineeManagement.Api.TrackTaskModel;
using TraineeManagement.Api.ExceptionUtils;
namespace TraineeManagement.Api.TrackTaskService;

public class TrackTaskService : ITrackTaskService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TrackTaskService>_logger;

    public TrackTaskService(AppDbContext context, ILogger<TrackTaskService> logger)
    {
        _context = context;
        _logger = logger;
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
            throw new NotFoundException($"Task Assignment not found");
        }
        return t;
    }

    public async Task<TrackTaskResponse> CreateTrackTaskAsync(TrackTaskRequestBody body)
    {
        if(body.DueDate < body.AssignedDate)
        {
            throw new BadRequestException("Due date cannot be earlier than assigned date");
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

        return ToResponse(trackTask);
    }

    public async Task<IEnumerable<TrackTaskResponse>> GetAllTasks()
    {
        IEnumerable<TrackTask> trackTasks = await _context.TrackTasks.ToListAsync();
        return trackTasks.Select(t => ToResponse(t));
    }

    public async Task<TrackTaskPopulatedResponseBody> GetTrackTaskByIdAsync(long id)
    {
        TrackTask? trackTask = await _context.TrackTasks
            .Include(t => t.Trainee)
            .Include(t => t.Mentor)
            .Include(t => t.LearningTask)
            .FirstOrDefaultAsync(t => t.Id == id) ; 
        
        if(trackTask is null)
        {
            throw new NotFoundException($"Task Assignment not found");
        }

        return new TrackTaskPopulatedResponseBody
        (
            Id: trackTask.Id,
            Trainee: new TraineeResponse(trackTask.Trainee.Id, trackTask.Trainee.FirstName, trackTask.Trainee.LastName, trackTask.Trainee.Email, trackTask.Trainee.TechStack, trackTask.Trainee.Status),
            Mentor: new MentorResponse(trackTask.Mentor.Id, trackTask.Mentor.FirstName,trackTask.Mentor.LastName, trackTask.Mentor.Email, trackTask.Mentor.Expertise, trackTask.Mentor.Status),
            LearningTask: new TaskResponseData(trackTask.LearningTask.Id, trackTask.LearningTask.Title, trackTask.LearningTask.Description, trackTask.LearningTask.ExpectedTechStack, trackTask.LearningTask.DueDate, trackTask.LearningTask.Status),
            AssignedDate: trackTask.AssignedDate,
            DueDate: trackTask.DueDate,
            Status: trackTask.Status,
            Remark: trackTask.Remark
        );
    }

    public async Task<TrackTaskResponse> UpdateTrackTaskAsync(long id, TrackTaskUpdateRequestBody body)
    {

        TrackTask trackTask = await FetchTrackTask(id);

        trackTask.Status = body.Status;

        await _context.SaveChangesAsync();

        return ToResponse(trackTask);
    }
}
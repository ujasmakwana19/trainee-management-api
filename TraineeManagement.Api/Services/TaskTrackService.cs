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
using TraineeManagement.WebCommons.AuthClaims;
using TraineeManagement.Data.UserModel;
using TraineeManagement.Data.MentorModel;
using TraineeManagement.Data.TraineeModel;
namespace TraineeManagement.Api.TrackTaskService;

public class TrackTaskService : ITrackTaskService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TrackTaskService>_logger;
    private readonly ICacheService _cache;

    private readonly ICurrentUserAccessor _currentUser ;

    public TrackTaskService(AppDbContext context, ICurrentUserAccessor currentUser, ILogger<TrackTaskService> logger, ICacheService cache)
    {
        _context = context;
        _currentUser = currentUser;
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

    public async Task<IEnumerable<TrackTaskPersonalResponse>> GetAllTasks()
    {
        long userId = _currentUser.Id;
        string userRole = _currentUser.Role;
        if(userRole == UserRole.Mentor.ToString())
        {
            Mentor? mentor = await _context.Mentors.AsNoTracking().FirstOrDefaultAsync(m => m.UserId == userId);

            if(mentor == null)
            {
                return [];
            }


            return await _context.TrackTasks.AsNoTracking()
                                            .Where(t => t.MentorId == mentor.Id)
                                            .Select(t => new TrackTaskPersonalResponse(
                                                t.Id,
                                                t.TraineeId,
                                                t.Trainee.FirstName + " "+t.Trainee.LastName,          
                                                t.MentorId,
                                                t.Mentor.FirstName + " "+t.Mentor.LastName,           
                                                t.LearningTaskId,
                                                t.LearningTask.Title,    
                                                t.AssignedDate,
                                                t.DueDate,
                                                t.Status,
                                                t.Remark
                                            ))
                                            .ToListAsync();
        }
        else if (userRole == UserRole.Trainee.ToString())
        {
            Trainee? trainee = await _context.Trainees.AsNoTracking().FirstOrDefaultAsync(t => t.UserId == userId);

            if(trainee == null)
            {
                return [];
            }


            return await _context.TrackTasks.AsNoTracking()
                                            .Where(t => t.TraineeId == trainee.Id)
                                            .Select(t => new TrackTaskPersonalResponse(
                                                t.Id,
                                                t.TraineeId,
                                                t.Trainee.FirstName + " "+t.Trainee.LastName,          
                                                t.MentorId,
                                                t.Mentor.FirstName + " "+t.Mentor.LastName,           
                                                t.LearningTaskId,
                                                t.LearningTask.Title,    
                                                t.AssignedDate,
                                                t.DueDate,
                                                t.Status,
                                                t.Remark
                                            ))
                                            .ToListAsync();
        }
        else if(userRole == UserRole.Admin.ToString())
        {
            return await _context.TrackTasks.AsNoTracking()
                                            .Select(t => new TrackTaskPersonalResponse(
                                                t.Id,
                                                t.TraineeId,
                                                t.Trainee.FirstName + " "+t.Trainee.LastName,          
                                                t.MentorId,
                                                t.Mentor.FirstName + " "+t.Mentor.LastName,           
                                                t.LearningTaskId,
                                                t.LearningTask.Title,    
                                                t.AssignedDate,
                                                t.DueDate,
                                                t.Status,
                                                t.Remark
                                            ))
                                            .ToListAsync();
        }
        else
        {
            return [];
        }
    }

    public async Task<TrackTaskPopulatedResponseBody> GetTrackTaskByIdAsync(long id)
    {
        long userId = _currentUser.Id;
        string userRole = _currentUser.Role;

        if(userRole == UserRole.Mentor.ToString())
        {
            Mentor? mentor = await _context.Mentors.AsNoTracking().FirstOrDefaultAsync(m => m.UserId == userId);

            if(mentor == null)
            {
                throw new NotFoundException(ErrorCodes.NOT_FOUND_TASK_ASSIGNMENT);
            }

            TrackTaskPopulatedResponseBody? t = await _context.TrackTasks.AsNoTracking()
                                            .Where(t => t.MentorId == mentor.Id && t.Id == id)
                                            .Select(t => new TrackTaskPopulatedResponseBody(
                                                t.Id,
                                                new TraineeResponse(
                                                    t.TraineeId,
                                                    t.Trainee.FirstName,
                                                    t.Trainee.LastName,
                                                    t.Trainee.Email,
                                                    t.Trainee.TechStack,
                                                    t.Trainee.Status
                                                ),
                                                new MentorResponse(
                                                    t.MentorId,
                                                    t.Mentor.FirstName,
                                                    t.Mentor.LastName,
                                                    t.Mentor.Email,
                                                    t.Mentor.Expertise,
                                                    t.Mentor.Status
                                                ),
                                                new TaskResponseData(
                                                    t.LearningTaskId,
                                                    t.LearningTask.Title,
                                                    t.LearningTask.Description,
                                                    t.LearningTask.ExpectedTechStack,
                                                    t.LearningTask.DueDate,
                                                    t.LearningTask.Status
                                                ),
                                                t.AssignedDate,
                                                t.DueDate,
                                                t.Status,
                                                t.Remark
                                            )).FirstOrDefaultAsync();

            if(t == null)
            {
                throw new NotFoundException(ErrorCodes.NOT_FOUND_TASK_ASSIGNMENT);
            }

            return t;

        }
        if(userRole == UserRole.Trainee.ToString())
        {
            Trainee? trainee = await _context.Trainees.AsNoTracking().FirstOrDefaultAsync(m => m.UserId == userId);

            if(trainee == null)
            {
                throw new NotFoundException(ErrorCodes.NOT_FOUND_TASK_ASSIGNMENT);
            }

            TrackTaskPopulatedResponseBody? t = await _context.TrackTasks.AsNoTracking()
                                            .Where(t => t.TraineeId == trainee.Id && t.Id == id)
                                            .Select(t => new TrackTaskPopulatedResponseBody(
                                                t.Id,
                                                new TraineeResponse(
                                                    t.TraineeId,
                                                    t.Trainee.FirstName,
                                                    t.Trainee.LastName,
                                                    t.Trainee.Email,
                                                    t.Trainee.TechStack,
                                                    t.Trainee.Status
                                                ),
                                                new MentorResponse(
                                                    t.MentorId,
                                                    t.Mentor.FirstName,
                                                    t.Mentor.LastName,
                                                    t.Mentor.Email,
                                                    t.Mentor.Expertise,
                                                    t.Mentor.Status
                                                ),
                                                new TaskResponseData(
                                                    t.LearningTaskId,
                                                    t.LearningTask.Title,
                                                    t.LearningTask.Description,
                                                    t.LearningTask.ExpectedTechStack,
                                                    t.LearningTask.DueDate,
                                                    t.LearningTask.Status
                                                ),
                                                t.AssignedDate,
                                                t.DueDate,
                                                t.Status,
                                                t.Remark
                                            )).FirstOrDefaultAsync();

            if(t == null)
            {
                throw new NotFoundException(ErrorCodes.NOT_FOUND_TASK_ASSIGNMENT);
            }

            return t;

        }
        else if(userRole == UserRole.Admin.ToString())
        {
            TrackTaskPopulatedResponseBody? t = await _context.TrackTasks.AsNoTracking()
                                            .Where(t => t.Id == id)
                                            .Select(t => new TrackTaskPopulatedResponseBody(
                                                t.Id,
                                                new TraineeResponse(
                                                    t.TraineeId,
                                                    t.Trainee.FirstName,
                                                    t.Trainee.LastName,
                                                    t.Trainee.Email,
                                                    t.Trainee.TechStack,
                                                    t.Trainee.Status
                                                ),
                                                new MentorResponse(
                                                    t.MentorId,
                                                    t.Mentor.FirstName,
                                                    t.Mentor.LastName,
                                                    t.Mentor.Email,
                                                    t.Mentor.Expertise,
                                                    t.Mentor.Status
                                                ),
                                                new TaskResponseData(
                                                    t.LearningTaskId,
                                                    t.LearningTask.Title,
                                                    t.LearningTask.Description,
                                                    t.LearningTask.ExpectedTechStack,
                                                    t.LearningTask.DueDate,
                                                    t.LearningTask.Status
                                                ),
                                                t.AssignedDate,
                                                t.DueDate,
                                                t.Status,
                                                t.Remark
                                            )).FirstOrDefaultAsync();

            if(t == null)
            {
                throw new NotFoundException(ErrorCodes.NOT_FOUND_TASK_ASSIGNMENT);
            }

            return t;
        }
        else
        {
            throw new NotFoundException(ErrorCodes.NOT_FOUND_TASK_ASSIGNMENT);
        }
        
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
using TraineeManagement.Api.TraineeModel;
using TraineeManagement.Api.TraineeDTO;
using TraineeManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.ExceptionUtils;
using TraineeManagement.Api.ErrorCodesUtils;
using TraineeManagement.Api.CacheServices;
using StackExchange.Redis;
namespace TraineeManagement.Api.TraineeServices;


public class TraineeService : ITraineeService
{
    // readonly makes sure that the list of trainees cannot be reassigned to a different list, but we can still add or remove items from the list. This is useful for maintaining the integrity of the data structure while allowing for modifications to the contents of the list.
    // private readonly List<Trainee> _trainees = new();

    // This is for the inMemory Database Instance
    private readonly AppDbContext _context;
    private readonly ILogger<TraineeService> _logger;
    private readonly ICacheService _cache;
    public TraineeService(AppDbContext context, ILogger<TraineeService> logger, ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    // Helper Method
    private static TraineeResponse ToResponse(Trainee trainee)
    {
        return new TraineeResponse(
            trainee.Id,
            trainee.FirstName,
            trainee.LastName,
            trainee.Email,
            trainee.TechStack,
            trainee.Status
        );
    }

    public async Task<Trainee> FetchTrainee(long id)
    {
        Trainee? trainee = await _context.Trainees.FirstOrDefaultAsync(u => u.Id == id);

        if (trainee is null)
        {
            throw new NotFoundException(ErrorCodes.NOT_FOUND_TRAINEE);
        }
        return trainee;
    }

    // DELETE
    public async Task DeleteTraineeService(long id)
    {
        Trainee t = await FetchTrainee(id);
        if (t is null)
        {
            throw new NotFoundException(ErrorCodes.NOT_FOUND_TRAINEE);
        }
        _context.Trainees.Remove(t);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Trainee with id {id} deleted successfully");
        return;
    }

    // GETALL
    public async Task<IEnumerable<TraineeResponse>> GetAllTraineesService()
    {
        IEnumerable<TraineeResponse> trainees = await _context.Trainees
                                                .Select(t => new TraineeResponse(
                                                    t.Id,
                                                    t.FirstName,
                                                    t.LastName,
                                                    t.Email,
                                                    t.TechStack,
                                                    t.Status
                                                ))
                                                .ToListAsync();
        return trainees;
    }

    // GET by ID
    public async Task<TraineeResponse> GetTraineeResponseByIdService(long id)
    {
        string cacheKey = $"trainee:{id}";
        TraineeResponse? trainee = await _cache.GetAsync<TraineeResponse>(cacheKey);
        if(trainee is null)
        {
            trainee = await _context.Trainees
                        .Where(t => t.Id == id)
                        .Select(t => new TraineeResponse(
                            t.Id,
                            t.FirstName,
                            t.LastName,
                            t.Email,
                            t.TechStack,
                            t.Status
                        ))
                        .FirstOrDefaultAsync();
            if (trainee is null)
            {
                throw new NotFoundException(ErrorCodes.NOT_FOUND_TRAINEE);
            }

            await _cache.SetAsync<TraineeResponse>(cacheKey, trainee, TimeSpan.FromMinutes(10));
            
        }
        return trainee;
    }

    // CREATE
    public async Task<TraineeResponse> CreateTraineeService(CreateTraineeRequest trainee)
    {
        Trainee u = new Trainee
        {
            FirstName = trainee.FirstName,
            LastName = trainee.LastName,
            Email = trainee.Email,
            TechStack = trainee.TechStack,
            Status = trainee.Status
        };

        _context.Trainees.Add(u);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Trainee created successfully"); 
        return ToResponse(u);
    }

    // UPDATE
    public async Task<TraineeResponse> UpdateTraineeService(long id, UpdateTraineeRequest trainee)
    {
        Trainee? user = await FetchTrainee(id);
        if (user is null)
        {
            throw new NotFoundException(ErrorCodes.NOT_FOUND_TRAINEE);
        }
        

        user.FirstName = trainee.FirstName;
        user.LastName = trainee.LastName;
        user.Email = trainee.Email;
        user.TechStack = trainee.TechStack;
        user.Status = trainee.Status;

        _context.Trainees.Update(user);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Trainee with id {id} updated successfully");

        return ToResponse(user);
    }

    // SEARCH
    public async Task<IEnumerable<TraineeResponse>> SearchTraineeService(String s)
    {
        s = s.ToLower();
        List<TraineeResponse>trainees = await _context.Trainees
                                            .Where(
                                                u => u.FirstName!.ToLower().Contains(s)
                                                || u.LastName!.ToLower().Contains(s)
                                                || u.TechStack!.ToLower().Contains(s)
                                                || u.Email!.ToLower().Contains(s))
                                            .Select(t => new TraineeResponse(
                                                t.Id,
                                                t.FirstName,
                                                t.LastName,
                                                t.Email,
                                                t.TechStack,
                                                t.Status
                                            )).ToListAsync();
        
        return trainees;
    }

    public async Task<TraineeInfoPagination> SearchTraineePaginationService(int pageNumber, int pageSize, String search, StatusValue status)
    {
        int rowToSkip = (pageNumber-1)*pageSize; 


        IQueryable<Trainee> query = _context.Trainees.AsNoTracking();

        query = query.Where(u =>
            u.FirstName == search &&
            u.Status == status
        );

        int totalRecords = await query.CountAsync();

        if (totalRecords == 0)
        {
            return new TraineeInfoPagination(
                pageNumber,
                pageSize,
                totalRecords,
                []
            );
        }
        
        List<TraineeResponse>trainees = await query
        .OrderBy(u => u.Id)
        .Skip(rowToSkip)
        .Take(pageSize)
        .Select(t => new TraineeResponse(
                    t.Id,
                    t.FirstName,
                    t.LastName,
                    t.Email,
                    t.TechStack,
                    t.Status
        ))
        .ToListAsync();

        return new TraineeInfoPagination(
            pageNumber,
            trainees.Count,
            totalRecords,
            trainees
        );
    }
};


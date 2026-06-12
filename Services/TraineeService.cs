using TraineeManagement.Api.TraineeModel;
using TraineeManagement.Api.TraineeDTO;
using TraineeManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.ExceptionUtils;


namespace TraineeManagement.Api.TraineeServices;


public class TraineeService : ITraineeService
{
    // readonly makes sure that the list of trainees cannot be reassigned to a different list, but we can still add or remove items from the list. This is useful for maintaining the integrity of the data structure while allowing for modifications to the contents of the list.
    // private readonly List<Trainee> _trainees = new();

    // This is for the inMemory Database Instance
    private readonly AppDbContext _context;
    private readonly ILogger<TraineeService> _logger;
    public TraineeService(AppDbContext context, ILogger<TraineeService> logger)
    {
        _context = context;
        _logger = logger;
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
            throw new NotFoundException("Trainee not found");
        }
        return trainee;
    }

    // DELETE
    public async Task DeleteTraineeService(long id)
    {
        Trainee t = await FetchTrainee(id);
        if (t is null)
        {
            throw new NotFoundException("Trainee not found");
        }
        _context.Trainees.Remove(t);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Trainee with id {id} deleted successfully");
        return;
    }

    // GETALL
    public async Task<IEnumerable<TraineeResponse>> GetAllTraineesService()
    {
        List<Trainee> trainees = await _context.Trainees.ToListAsync();
        return trainees.Select(t => ToResponse(t));
    }

    // GET by ID
    public async Task<TraineeResponse> GetTraineeResponseByIdService(long id)
    {
        Trainee trainee = await FetchTrainee(id);
        if (trainee is null)
        {
            throw new NotFoundException("Trainee not found");
        }
        return ToResponse(trainee);
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
            throw new NotFoundException("Trainee not found");
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
        List<Trainee>trainees = await _context.Trainees.Where(
            u => u.FirstName!.ToLower().Contains(s)
            || u.LastName!.ToLower().Contains(s)
            || u.TechStack!.ToLower().Contains(s)
            || u.Email!.ToLower().Contains(s)).ToListAsync();
        
        return trainees.Select(u => ToResponse(u));
    }

    public async Task<TraineeInfoPagination> SearchTraineePaginationService(int pageNumber, int pageSize, String search, String status)
    {
        if(pageNumber < 1) pageNumber = 1;
        if(pageSize < 1) pageSize = 10;

        int rowToSkip = (pageNumber-1)*pageSize; 

        int totalRecords = await _context.Trainees
        .Where(
            u => u.FirstName!.ToLower().Equals(search) &&
            u.Status.ToString()!.ToLower().Equals(status)
        ).CountAsync();

        if (totalRecords == 0)
        {
            return new TraineeInfoPagination(
                pageNumber,
                pageSize,
                totalRecords,
                []
            );
        }
        
        List<Trainee>trainees = await _context.Trainees
        .OrderBy(u => u.Id)
        .Skip(rowToSkip)
        .Where(
            u => u.FirstName!.ToLower().Equals(search) &&
            u.Status.ToString()!.ToLower().Equals(status)
        ).Take(pageSize)
        .ToListAsync();

        
        List<TraineeResponse> tr = trainees.Select(trainees => ToResponse(trainees)).ToList();

        return new TraineeInfoPagination(
            pageNumber,
            trainees.Count,
            totalRecords,
            tr
        );;
    }
};


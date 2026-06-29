using TraineeManagement.Data.TraineeModel;
using TraineeManagement.Data.TraineeDTO;
using TraineeManagement.Data.DataBaseContext;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Contracts.ExceptionUtils;
using TraineeManagement.Contracts.ErrorCodesUtils;
using TraineeManagement.Data.CacheServices;
using System.Text.Json;
using TraineeManagement.Data.ResponseDTO;
using NuGet.Protocol;
using System.Net;
using TraineeManagement.Contracts.CoorealationIdServices;
using TraineeManagement.Contracts.CoorealationIdMiddlewares;
namespace TraineeManagement.Api.TraineeServices;


public class TraineeService : ITraineeService
{
    // readonly makes sure that the list of trainees cannot be reassigned to a different list, but we can still add or remove items from the list. This is useful for maintaining the integrity of the data structure while allowing for modifications to the contents of the list.
    // private readonly List<Trainee> _trainees = new();

    // This is for the inMemory Database Instance
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly AppDbContext _context;
    private readonly ILogger<TraineeService> _logger;
    private readonly ICacheService _cache;
    private readonly ICorrelationIdAccessor _correlationIdAccessor;
    public TraineeService(IHttpClientFactory httpClientFactory, IConfiguration config, AppDbContext context, ILogger<TraineeService> logger, ICacheService cache, ICorrelationIdAccessor correlationIdAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
        _context = context;
        _logger = logger;
        _cache = cache;
        _correlationIdAccessor = correlationIdAccessor;
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
    public async Task<TraineeResponse> GetTraineeResponseByIdService(long id, CancellationToken cancellationToken)
    {
        string cacheKey = CacheKey.traineeId + $"{id}";
        TraineeResponse? trainee = await _cache.GetAsync<TraineeResponse>(cacheKey);
        if (trainee is not null)
        {
            return trainee;
        }

        string clientName = _config["TraineeMicroService:NAME"]!;
        HttpClient client = _httpClientFactory.CreateClient(clientName);

        using HttpRequestMessage request = new(HttpMethod.Get, $"trainees/{id}");

        string? correlationId = _correlationIdAccessor.CorrelationId;
        if (!string.IsNullOrEmpty(correlationId))
        {
            request.Headers.TryAddWithoutValidation(CorrelationIdMiddleware.HeaderName, correlationId);
        }

        using HttpResponseMessage response = await client.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new NotFoundException(ErrorCodes.NOT_FOUND_TRAINEE);
        }

        
        response.EnsureSuccessStatusCode(); 

        InterCommunicationResponse<TraineeResponse>? responseData =
            await response.Content.ReadFromJsonAsync<InterCommunicationResponse<TraineeResponse>>(
                new JsonSerializerOptions(JsonSerializerDefaults.Web),
                cancellationToken);

        if (responseData is null || responseData.Success == false || responseData.Data is null)
        {
            throw new NotFoundException(ErrorCodes.NOT_FOUND_TRAINEE);
        }

        trainee = responseData.Data;
        await _cache.SetAsync(cacheKey, trainee, CacheTTL.GETS_TTL_MIN);
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
        
        await _cache.RemoveAsync(CacheKey.traineeId + $"{id}");
        TraineeResponse t = ToResponse(user);
        await _cache.SetAsync<TraineeResponse>(CacheKey.traineeId + $"{id}", t, CacheTTL.GETS_TTL_MIN);

        return t;
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


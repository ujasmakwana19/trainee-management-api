using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data.CacheServices;
using TraineeManagement.Data.DataBaseContext;
using TraineeManagement.WebCommons.ErrorCodesUtils;
using TraineeManagement.WebCommons.ExceptionUtils;
using TraineeManagement.Data.MentorDTO;
using TraineeManagement.Data.MentorModel;
using Microsoft.AspNetCore.Identity;
using TraineeManagement.Data.UserModel;
namespace TraineeManagement.Api.MentorServices;

public class MentorService : IMentorService
{
    private readonly AppDbContext _context;
    private readonly ILogger<MentorService> _logger;
    private readonly ICacheService _cache;

    public MentorService(AppDbContext context, ILogger<MentorService> logger, ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    private static MentorResponse ToResponse(Mentor mentor)
    {
        return new MentorResponse(
            mentor.Id,
            mentor.FirstName,
            mentor.LastName,
            mentor.Email,
            mentor.Expertise,
            mentor.Status
        );
    }

    private async Task<Mentor> FetchMentor(long id)
    {
        Mentor? mentor = await _context.Mentors.FirstOrDefaultAsync(m => m.Id == id);
        if(mentor is null)
        {
            throw new NotFoundException(ErrorCodes.NOT_FOUND_MENTOR);
        }
        return mentor;
    }

    // GETALL
    public async Task<IEnumerable<MentorResponse>> GetAll()
    {
        string cacheKey = CacheKey.mentorall;
        IEnumerable<MentorResponse>? mentors = await _cache.GetAsync<IEnumerable<MentorResponse>>(cacheKey); 

        if(mentors is null)
        {    
            mentors = await _context.Mentors
                                    .Select(t => new MentorResponse(
                                        t.Id,
                                        t.FirstName,
                                        t.LastName,
                                        t.Email,
                                        t.Expertise,
                                        t.Status
                                    ))
                                    .ToListAsync();
            if(mentors.Any())
                await _cache.SetAsync<IEnumerable<MentorResponse>>(cacheKey, mentors, CacheTTL.GETS_TTL_MIN);
        }
        return mentors;
    }

    // GET by ID
    public async Task<MentorResponse> GetById(long id)
    {
        string cacheKey = CacheKey.mentorall;
        IEnumerable<MentorResponse>? mentors = await _cache.GetAsync<IEnumerable<MentorResponse>>(cacheKey);
        MentorResponse? mentor = mentors?.FirstOrDefault(t => t.Id == id);
        if(mentor is null)
        {   
            mentor = await _context.Mentors
                                .Where(t => t.Id == id)
                                .Select(t => new MentorResponse(
                                    t.Id,
                                    t.FirstName,
                                    t.LastName,
                                    t.Email,
                                    t.Expertise,
                                    t.Status
                                ))
                                .FirstOrDefaultAsync();
            if(mentor is null)
                throw new NotFoundException(ErrorCodes.NOT_FOUND_MENTOR);

        }
        return mentor;
    }

    // CREATE
    public async Task<MentorResponse> CreateMentor(MentorRequestBody mentorInfo)
    {
        try
        {
            PasswordHasher<User> ph = new PasswordHasher<User>();
            Random random = new Random();

            User user = new User
            {
                Username = $"{mentorInfo.FirstName}{mentorInfo.LastName}{random.Next(1, 999)}",
                Email = mentorInfo.Email,
                Role = UserRole.Mentor
            };

            user.PasswordHash = ph.HashPassword(user, mentorInfo.Password);

            Mentor mentor = new Mentor
            {
                FirstName = mentorInfo.FirstName,
                LastName = mentorInfo.LastName,
                Email = mentorInfo.Email,
                Expertise = mentorInfo.Expertise,
                Status = mentorInfo.Status,
                User = user
            };

            _context.Mentors.Add(mentor);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Mentor {MentorId} created successfully", mentor.Id);
            await _cache.RemoveAsync(CacheKey.mentorall);
            return ToResponse(mentor);
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationFailed(ex, ErrorCodes.MENTOR_NOT_CREATED);
        }
    }

    // UPDATE
    public async Task<MentorResponse> UpdateMentor(long Id,MentorRequestBody mentorInfo)
    {
        Mentor mentor = await FetchMentor(Id);
        
        mentor.FirstName = mentorInfo.FirstName;
        mentor.LastName = mentorInfo.LastName;
        mentor.Email = mentorInfo.Email;
        mentor.Expertise = mentorInfo.Expertise;
        mentor.Status = mentorInfo.Status;
        

        _context.Mentors.Update(mentor);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Mentor {MentorId} updated successfully", mentor.Id);
        await _cache.RemoveAsync(CacheKey.mentorall);
        return ToResponse(mentor);
    }

    // DELETE
    public async Task DeleteMentor(long id)
    {
        Mentor mentor = await FetchMentor(id);

        _context.Mentors.Remove(mentor);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Mentor {MentorId} deleted successfully", mentor.Id);
        await _cache.RemoveAsync(CacheKey.mentorall);
        return;
    }
}
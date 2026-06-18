using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.ErrorCodesUtils;
using TraineeManagement.Api.ExceptionUtils;
using TraineeManagement.Api.MentorDTO;
using TraineeManagement.Api.MentorModel;
namespace TraineeManagement.Api.MentorServices;

public class MentorService : IMentorService
{
    private readonly AppDbContext _context;
    private readonly ILogger<MentorService> _logger;

    public MentorService(AppDbContext context, ILogger<MentorService> logger)
    {
        _context = context;
        _logger = logger;
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
        IEnumerable<MentorResponse> mentors = await _context.Mentors
                                .Select(t => new MentorResponse(
                                    t.Id,
                                    t.FirstName,
                                    t.LastName,
                                    t.Email,
                                    t.Expertise,
                                    t.Status
                                ))
                                .ToListAsync();
        return mentors;
    }

    // GET by ID
    public async Task<MentorResponse> GetById(long id)
    {
        MentorResponse?  mentor = await _context.Mentors
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
        return mentor;
    }

    // CREATE
    public async Task<MentorResponse> CreateMentor(MentorRequestBody mentorInfo)
    {

        Mentor mentor = new Mentor
        {
            FirstName = mentorInfo.FirstName,
            LastName = mentorInfo.LastName,
            Email = mentorInfo.Email,
            Expertise = mentorInfo.Expertise,
            Status = mentorInfo.Status
        };

        _context.Mentors.Add(mentor);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Mentor {MentorId} created successfully", mentor.Id);
        return ToResponse(mentor);
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
        return ToResponse(mentor);
    }

    // DELETE
    public async Task DeleteMentor(long id)
    {
        Mentor mentor = await FetchMentor(id);

        _context.Mentors.Remove(mentor);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Mentor {MentorId} deleted successfully", mentor.Id);
        return;
    }
}
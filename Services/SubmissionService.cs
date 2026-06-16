using Mapster;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.ExceptionUtils;
using TraineeManagement.Api.SubmissionDTO;
using TraineeManagement.Api.SubmissionModel;

namespace TraineeManagement.Api.SubmissionService;

public class SubmissionService : ISubmissionService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SubmissionService> _logger;

    public SubmissionService(AppDbContext context, ILogger<SubmissionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    private async Task<Submission> FetchSubmission(long id)
    {
        Submission? s = await _context.Submissions.FirstOrDefaultAsync(t => t.Id == id);

        if(s is null)
            throw new NotFoundException("Submission Not Found");

        return s;
    }

    private SubmissionResponse ToResponse(Submission task)
    {
        return new SubmissionResponse(
            task.Id,
            task.TaskAssignmentId,
            task.SubmissionUrl,
            task.Notes,
            task.SubmittedDate,
            task.Status
        );
    }

    public async Task<SubmissionResponse> CreateSubmission(SubmissionRequestBody body)
    {
        Submission s = new Submission
        {
            TaskAssignmentId = body.TaskAssignmentId,
            SubmissionUrl = body.SubmissionUrl,
            Notes = body.Notes,
            SubmittedDate = body.SubmittedDate,
            Status = body.Status   
        };
        _context.Submissions.Add(s);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Submission {SubmissionId} created successfully", s.Id);
        return ToResponse(s);
    }

    public async Task<SubmissionResponse> GetSubmissionById(long id)
    {
        Submission s = await FetchSubmission(id);
        return ToResponse(s);
    }

    public async Task<IEnumerable<SubmissionResponse>> GetAll()
    {
        List<SubmissionResponse> submissions = await _context.Submissions
                                        .ProjectToType<SubmissionResponse>()
                                        .ToListAsync();

        return submissions;
    }
}
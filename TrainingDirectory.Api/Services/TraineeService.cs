using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data.DataBaseContext;
using TraineeManagement.Contracts.ErrorCodesUtils;
using TraineeManagement.Contracts.ExceptionUtils;
using TraineeManagement.Data.TraineeDTO;
using TrainingDirectory.TraineeInterface;

namespace TrainingDirectory.TraineeServices;
public class TraineeService : ITraineeService
{
    private readonly AppDbContext _context;
    
    public TraineeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TraineeResponse> GetById(long id)
    {
        TraineeResponse? trainee = trainee = await _context.Trainees
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
        return trainee;    
    }
}
using TraineeManagement.Api.Models;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Threading.Tasks;
using Namotion.Reflection;

namespace TraineeManagement.Api.Services;


public class TraineeService : ITraineeService
{
    // readonly makes sure that the list of trainees cannot be reassigned to a different list, but we can still add or remove items from the list. This is useful for maintaining the integrity of the data structure while allowing for modifications to the contents of the list.
    // private readonly List<Trainee> _trainees = new();

    // This is for the inMemory Database Instance
    private readonly AppDbContext _context;
    public TraineeService(AppDbContext context)
    {
        _context = context;
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

    public async Task<Trainee?> FetchTrainee(long id)
    {
        Trainee? trainee = await _context.Trainees.FirstOrDefaultAsync(u => u.Id == id);

        if (trainee == null)
        {
            return null;
        }
        return trainee;
    }

    // DELETE
    public async Task<bool> DeleteTraineeService(long id)
    {
        Trainee? t = await FetchTrainee(id);
        if (t is null)
        {
            return false;
        }
        _context.Trainees.Remove(t);
        await _context.SaveChangesAsync();
        return true;
    }

    // GETALL
    public async Task<IEnumerable<TraineeResponse>?> GetAllTraineesService()
    {
        List<Trainee> trainees = await _context.Trainees.ToListAsync();
        if (trainees.Count == 0)
        {
            return null;
        }

        return trainees.Select(t => ToResponse(t));
    }

    // GET by ID
    public async Task<TraineeResponse?> GetTraineeResponseByIdService(long id)
    {
        Trainee? trainee = await FetchTrainee(id);
        if (trainee is null)
        {
            return null;
        }
        return ToResponse(trainee);
    }

    // CREATE
    public async Task<TraineeResponse?> CreateTraineeService(CreateTraineeRequest trainee)
    {

        Trainee u = new Trainee
        {
            FirstName = trainee.FirstName,
            LastName = trainee.LastName,
            Email = trainee.Email,
            TechStack = trainee.TechStack,
            Status = trainee.Status,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        _context.Trainees.Add(u);
        await _context.SaveChangesAsync();

        return ToResponse(u);
    }

    // UPDATE
    public async Task<TraineeResponse?> UpdateTraineeService(long id, UpdateTraineeRequest trainee)
    {
        Trainee? user = await FetchTrainee(id);
        if (user is null)
        {
            return null;
        }

        user.FirstName = trainee.FirstName;
        user.LastName = trainee.LastName;
        user.Email = trainee.Email;
        user.TechStack = trainee.TechStack;
        user.Status = trainee.Status;
        user.UpdatedDate = DateTime.UtcNow;

        _context.Trainees.Update(user);
        await _context.SaveChangesAsync();

        return ToResponse(user);
    }

    // SEARCH
    public async Task<IEnumerable<TraineeResponse>?> SearchTraineeService(String s)
    {
        s.ToLower();
        List<Trainee>trainees = await _context.Trainees.Where(
            u => u.FirstName!.ToLower().Contains(s)
            || u.LastName!.ToLower().Contains(s)
            || u.TechStack!.ToLower().Contains(s)
            || u.Email!.ToLower().Contains(s)).ToListAsync();

        if (trainees.Count == 0)
        {
            return null;
        }
        
        return trainees.Select(u => ToResponse(u));
    }
};


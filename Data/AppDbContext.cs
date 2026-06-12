using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.TraineeModel;
using TraineeManagement.Api.UserModel;
using TraineeManagement.Api.IDateTimeAutoService;
using TraineeManagement.Api.MentorModel;
using TraineeManagement.Api.TaskModel;
namespace TraineeManagement.Api.Data;

public class AppDbContext : DbContext
{    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Store the Enum as a String "Admin" as defined in the Enum
        builder.Entity<User>()
        .Property(u => u.Role)
        .HasConversion<string>();

        builder.Entity<Trainee>()
        .Property(u => u.Status)
        .HasConversion<string>();

        builder.Entity<Mentor>()
        .Property(u => u.Status)
        .HasConversion<string>();

        builder.Entity<LearningTask>()
        .Property(u => u.Status)
        .HasConversion<string>();

    }
    

    // This runs on every INSERT and UPDATE
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<IDateTimeAuto>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDate = DateTime.UtcNow;
                entry.Entity.UpdatedDate = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedDate = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    
    }
    
    public DbSet<Trainee> Trainees { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Mentor> Mentors {get; set;}
    public DbSet<LearningTask> LearningTasks {get; set;}

}
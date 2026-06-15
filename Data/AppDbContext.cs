using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.TraineeModel;
using TraineeManagement.Api.UserModel;
using TraineeManagement.Api.IDateTimeAutoService;
using TraineeManagement.Api.MentorModel;
using TraineeManagement.Api.TaskModel;
using TraineeManagement.Api.TrackTaskModel;
using TraineeManagement.Api.SubmissionModel;
using TraineeManagement.Api.ReviewModel;
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
        .HasIndex(u => u.Username)
        .IsUnique();

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

        builder.Entity<TrackTask>(entity =>
        {
            entity.HasOne(t => t.Trainee)
                .WithMany()
                .HasForeignKey(t => t.TraineeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Mentor)
                .WithMany()
                .HasForeignKey(t => t.MentorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.LearningTask)
                .WithMany()
                .HasForeignKey(t => t.LearningTaskId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(u => u.Status)
                .HasConversion<string>();
        });

        builder.Entity<Submission>(entity =>
        {
            entity.HasOne(t => t.TrackTask)
                .WithMany()
                .HasForeignKey(t => t.TaskAssignmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(u => u.Status)
                .HasConversion<string>();
        });

        builder.Entity<Review>(entity =>
        {
            entity.HasOne(t => t.Submission)
                .WithMany()
                .HasForeignKey(t => t.SubmissionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Mentor)
                .WithMany()
                .HasForeignKey(t => t.MentorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(u => u.ReviewStatus)
                .HasConversion<string>();
        });

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
    public DbSet<TrackTask> TrackTasks {get; set;}
    public DbSet<Submission> Submissions {get; set;}
    public DbSet<Review> Reviews {get; set;}

}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TraineeManagement.Data.TraineeModel;
using TraineeManagement.Data.UserModel;
using TraineeManagement.Data.IDateTimeAutoService;
using TraineeManagement.Data.MentorModel;
using TraineeManagement.Data.TaskModel;
using TraineeManagement.Data.TrackTaskModel;
using TraineeManagement.Data.SubmissionModel;
using TraineeManagement.Data.ReviewModel;
using TraineeManagement.Data.SubmissionFileModel;
using TraineeManagement.Data.ProcessingJobModel;

namespace TraineeManagement.Data.DataBaseContext;

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

        builder.Entity<Trainee>()
        .HasIndex(u => new { u.FirstName, u.Status });


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

        });

        builder.Entity<Submission>(entity =>
        {
            entity.HasOne(t => t.TrackTask)
                .WithMany()
                .HasForeignKey(t => t.TaskAssignmentId)
                .OnDelete(DeleteBehavior.Restrict);

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
        });

        builder.Entity<SubmissionFile>(entity =>
        {
            entity.HasOne(t => t.Submission)
                .WithMany()
                .HasForeignKey(t => t.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

        });

    }
    

    // This runs on every INSERT and UPDATE
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<EntityEntry<IDateTimeAuto>> entries = ChangeTracker.Entries<IDateTimeAuto>();

        foreach (EntityEntry<IDateTimeAuto> entry in entries)
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
    public DbSet<SubmissionFile> SubmissionFiles {get; set;}
    public DbSet<ProcessingJob> ProcessingJobs {get; set;}

}
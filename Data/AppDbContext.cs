using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.TraineeModel;
using TraineeManagement.Api.UserModel;
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

    }
    public DbSet<Trainee> Trainees { get; set; }
    public DbSet<User> Users { get; set; }

}
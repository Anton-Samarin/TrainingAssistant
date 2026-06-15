using Microsoft.EntityFrameworkCore;
using TrainingAssistant.Domain.Entities;

namespace TrainingAssistant.Infrastructure.Persistence;

/// <summary>
/// Контекст Entity Framework для PostgreSQL
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<FoodItem> FoodItems => Set<FoodItem>();
    public DbSet<WeeklyPlan> WeeklyPlans => Set<WeeklyPlan>();
    public DbSet<NutritionDay> NutritionDays => Set<NutritionDay>();
    public DbSet<Meal> Meals => Set<Meal>();
    public DbSet<MealItem> MealItems => Set<MealItem>();
    public DbSet<TrainingDay> TrainingDays => Set<TrainingDay>();
    public DbSet<TrainingExercise> TrainingExercises => Set<TrainingExercise>();
    public DbSet<UserExercisePreference> UserExercisePreferences => Set<UserExercisePreference>();
    public DbSet<BodyMetricLog> BodyMetricLogs => Set<BodyMetricLog>();
    public DbSet<StrengthRecord> StrengthRecords => Set<StrengthRecord>();
    public DbSet<ProgressNote> ProgressNotes => Set<ProgressNote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Name).HasMaxLength(80);
            e.Property(x => x.Email).HasMaxLength(256);
        });

        modelBuilder.Entity<UserProfile>(e =>
        {
            e.HasKey(x => x.UserId);
            e.HasOne(x => x.User).WithOne(x => x.Profile).HasForeignKey<UserProfile>(x => x.UserId);
            e.Property(x => x.WeightKg).HasPrecision(5, 2);
            e.Property(x => x.TrainingFocus).HasDefaultValue(Domain.Enums.TrainingFocus.Mixed);
        });

        modelBuilder.Entity<FoodItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<WeeklyPlan>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.UserId, x.IsActive });
            e.HasOne(x => x.User).WithMany(x => x.WeeklyPlans).HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<NutritionDay>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.WeeklyPlanId, x.DayIndex }).IsUnique();
            e.HasOne(x => x.WeeklyPlan).WithMany(x => x.NutritionDays).HasForeignKey(x => x.WeeklyPlanId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Meal>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.NutritionDay).WithMany(x => x.Meals).HasForeignKey(x => x.NutritionDayId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MealItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Meal).WithMany(x => x.Items).HasForeignKey(x => x.MealId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.FoodItem).WithMany().HasForeignKey(x => x.FoodItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TrainingDay>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.WeeklyPlanId, x.DayIndex }).IsUnique();
            e.HasOne(x => x.WeeklyPlan).WithMany(x => x.TrainingDays).HasForeignKey(x => x.WeeklyPlanId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TrainingExercise>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.TrainingDay).WithMany(x => x.Exercises).HasForeignKey(x => x.TrainingDayId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserExercisePreference>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.UserId, x.AvoidExerciseName }).IsUnique();
            e.Property(x => x.AvoidExerciseName).HasMaxLength(120);
            e.Property(x => x.PreferredExerciseName).HasMaxLength(120);
            e.Property(x => x.PoolKey).HasMaxLength(32);
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BodyMetricLog>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.UserId, x.LogDate });
            e.Property(x => x.WeightKg).HasPrecision(5, 2);
            e.Property(x => x.Note).HasMaxLength(500);
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StrengthRecord>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.UserId, x.RecordDate });
            e.Property(x => x.ExerciseName).HasMaxLength(120);
            e.Property(x => x.WeightKg).HasPrecision(6, 2);
            e.Property(x => x.Note).HasMaxLength(500);
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProgressNote>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.UserId, x.NoteDate });
            e.Property(x => x.Text).HasMaxLength(4000);
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}

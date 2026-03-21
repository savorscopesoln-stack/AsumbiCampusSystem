using Microsoft.EntityFrameworkCore;   // ← must be at the top
using AsumbiCampusSystem.Models;        // ← must be at the top

namespace AsumbiCampusSystem.Data
{
    public class AppDbContextNew : DbContext
    {
        public AppDbContextNew(DbContextOptions<AppDbContextNew> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<MealRecord> MealRecords { get; set; }
        public DbSet<StaffMember> StaffMembers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<MealCard> MealCards { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<LeaveRecord> LeaveRecords { get; set; }
        public DbSet<SchoolClass> SchoolClasses { get; set; }
        public DbSet<RFIDLog> RFIDLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure MealRecord defaults
            modelBuilder.Entity<MealRecord>(entity =>
            {
                entity.Property(m => m.MealType)
                      .HasDefaultValue("Breakfast");

                entity.Property(m => m.Status)
                      .HasDefaultValue("Served");

                entity.Property(m => m.Date)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("GETDATE()");
            });

            // Seed master admin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "masteradmin",
                    Password = "1234",
                    Role = "Master Admin",
                    StudentId = null
                }
            );
        }
    }
}
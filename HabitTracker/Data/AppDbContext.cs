using Microsoft.EntityFrameworkCore;
using HabitTracker.Models;

namespace HabitTracker.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Habit> Habits { get; set; }
        public DbSet<UserSetting> UserSettings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<HabitType> HabitTypes { get; set; }
        public DbSet<HabitStatus> HabitStatuses { get; set; }
        public DbSet<FrequencyType> FrequencyTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=habit_tracker.db");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HabitTracker.Models
{
    /// <summary>
    /// Пользователь приложения.
    /// </summary>
    [Table("USERS")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public long UserId { get; set; }

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("username")]
        public string Username { get; set; } = string.Empty;

        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("last_login_at")]
        public DateTime? LastLoginAt { get; set; }

        [Column("is_active")]
        public bool? IsActive { get; set; }

        // Навигационные свойства
        public List<Habit> Habits { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public UserSetting? UserSetting { get; set; }
    }

    /// <summary>
    /// Настройки пользователя.
    /// </summary>
    [Table("USER_SETTINGS")]
    public class UserSetting
    {
        [Key]
        [Column("setting_id")]
        public long SettingId { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [Column("theme")]
        public string? Theme { get; set; }

        [Column("language")]
        public string? Language { get; set; }

        [Column("notifications_enabled")]
        public bool? NotificationsEnabled { get; set; }

        [Column("start_of_week")]
        public int? StartOfWeek { get; set; }

        [Column("timezone")]
        public string? Timezone { get; set; }

        [Column("avatar_path")]
        public string? AvatarPath { get; set; }

        public User? User { get; set; }
    }

    /// <summary>
    /// Категория привычек.
    /// </summary>
    [Table("CATEGORIES")]
    public class Category
    {
        [Key]
        [Column("category_id")]
        public long CategoryId { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("color")]
        public string? Color { get; set; }

        [Column("icon")]
        public string? Icon { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        public User? User { get; set; }
        public List<Habit> Habits { get; set; } = new();
    }

    /// <summary>
    /// Привычка.
    /// </summary>
    [Table("HABITS")]
    public class Habit
    {
        [Key]
        [Column("habit_id")]
        public long HabitId { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [Column("category_id")]
        public long? CategoryId { get; set; }

        [Column("habit_type_id")]
        public int HabitTypeId { get; set; }

        [Column("status_id")]
        public int StatusId { get; set; }

        [Column("frequency_type_id")]
        public int FrequencyTypeId { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("target_value")]
        public decimal? TargetValue { get; set; }

        [Column("target_unit")]
        public string? TargetUnit { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        // Навигационные свойства
        public User? User { get; set; }
        public Category? Category { get; set; }
        public HabitType? HabitType { get; set; }
        public HabitStatus? HabitStatus { get; set; }
        public FrequencyType? FrequencyType { get; set; }
    }

    /// <summary>
    /// Справочник типов привычек (Полезная/Вредная).
    /// </summary>
    [Table("HABIT_TYPES")]
    public class HabitType
    {
        [Key]
        [Column("habit_type_id")]
        public int HabitTypeId { get; set; }

        [Column("code")]
        public string Code { get; set; } = string.Empty;

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        public List<Habit> Habits { get; set; } = new();
    }

    /// <summary>
    /// Справочник статусов привычек.
    /// </summary>
    [Table("HABIT_STATUSES")]
    public class HabitStatus
    {
        [Key]
        [Column("status_id")]
        public int StatusId { get; set; }

        [Column("code")]
        public string Code { get; set; } = string.Empty;

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        public List<Habit> Habits { get; set; } = new();
    }

    /// <summary>
    /// Справочник периодичности (Ежедневно, Еженедельно).
    /// </summary>
    [Table("FREQUENCY_TYPES")]
    public class FrequencyType
    {
        [Key]
        [Column("frequency_type_id")]
        public int FrequencyTypeId { get; set; }

        [Column("code")]
        public string Code { get; set; } = string.Empty;

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        public List<Habit> Habits { get; set; } = new();
    }
}
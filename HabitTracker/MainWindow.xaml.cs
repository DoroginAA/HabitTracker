using System;
using System.Windows;
using HabitTracker.Data;
using HabitTracker.Models;

namespace HabitTracker
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Инициализируем БД при запуске окна
            InitializeDatabase();
        }

        /// <summary>
        /// Метод для создания базы данных и заполнения справочников.
        /// </summary>
        private void InitializeDatabase()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    // Создаем БД, если её нет
                    if (context.Database.EnsureCreated())
                    {
                        // Если БД создана только что, заполняем справочники
                        context.HabitTypes.AddRange(
                            new HabitType { Code = "GOOD", Name = "Полезная" },
                            new HabitType { Code = "BAD", Name = "Вредная" }
                        );

                        context.HabitStatuses.AddRange(
                            new HabitStatus { Code = "ACTIVE", Name = "Активная" },
                            new HabitStatus { Code = "ARCHIVED", Name = "В архиве" }
                        );

                        context.FrequencyTypes.AddRange(
                            new FrequencyType { Code = "DAILY", Name = "Ежедневно" },
                            new FrequencyType { Code = "WEEKLY", Name = "Еженедельно" }
                        );

                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                // Если произошла ошибка, показываем её на экране
                MessageBox.Show($"Ошибка при инициализации БД: {ex.Message}\n Внутренняя ошибка: {ex.InnerException?.Message}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
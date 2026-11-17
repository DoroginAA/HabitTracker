using System;
using System.Windows;
using HabitTracker.Data;
using HabitTracker.Models;
using HabitTracker.Views;

namespace HabitTracker
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Создаем базу данных и справочники
            try
            {
                using (var context = new AppDbContext())
                {
                    if (context.Database.EnsureCreated())
                    {
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
                MessageBox.Show($"Ошибка инициализации БД:\n{ex.Message}", "Критическая ошибка");
                Shutdown();
                return;
            }
        }
    }
}
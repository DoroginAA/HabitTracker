using HabitTracker.Data;
using HabitTracker.Factories;
using HabitTracker.Models;
using HabitTracker.Repositories;
using HabitTracker.Services;
using HabitTracker.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace HabitTracker
{
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

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

            var serviceCollection = new ServiceCollection();

            // Регистрируем зависимости
            serviceCollection.AddTransient<IHabitRepository, HabitRepository>();
            serviceCollection.AddTransient<IHabitFactory, HabitFactory>();
            serviceCollection.AddTransient<HabitService>();

            ServiceProvider = serviceCollection.BuildServiceProvider();

            // Запускаем окно логина
            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}
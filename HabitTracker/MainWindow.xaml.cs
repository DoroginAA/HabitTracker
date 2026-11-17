using System;
using System.Linq;
using System.Windows;
using HabitTracker.Data;
using HabitTracker.Models;
using System.Windows.Controls;

namespace HabitTracker
{
    public partial class MainWindow : Window
    {
        private long _currentUserId;

        public MainWindow(long userId)
        {
            InitializeComponent();
            _currentUserId = userId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadHabits();
        }

        private void LoadHabits()
        {
            using (var context = new AppDbContext())
            {
                // Загружаем привычки текущего пользователя
                var habits = context.Habits.Where(h => h.UserId == _currentUserId).ToList();
                HabitsListBox.ItemsSource = habits;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string habitName = TxtHabitName.Text.Trim();

            if (string.IsNullOrWhiteSpace(habitName))
            {
                MessageBox.Show("Введите название!", "Внимание");
                return;
            }

            try
            {
                using (var context = new AppDbContext())
                {
                    var newHabit = new Habit
                    {
                        Name = habitName,
                        UserId = _currentUserId,
                        HabitTypeId = 1,
                        StatusId = 1,
                        FrequencyTypeId = 1,
                        StartDate = DateTime.Now
                    };

                    context.Habits.Add(newHabit);
                    context.SaveChanges();
                }

                TxtHabitName.Text = string.Empty;
                LoadHabits();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        // ОБРАБОТЧИК УДАЛЕНИЯ
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Получаем ID привычки из Tag кнопки, на которую нажали
            if (sender is Button button && button.Tag is long habitId)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить эту привычку?",
                                             "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var context = new AppDbContext())
                        {
                            var habit = context.Habits.FirstOrDefault(h => h.HabitId == habitId);
                            if (habit != null)
                            {
                                context.Habits.Remove(habit);
                                context.SaveChanges();
                            }
                        }
                        LoadHabits(); // Обновляем список
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                    }
                }
            }
        }
    }
}
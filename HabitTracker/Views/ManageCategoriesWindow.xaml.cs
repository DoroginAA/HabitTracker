using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HabitTracker.Data;
using HabitTracker.Models;

namespace HabitTracker.Views
{
    public partial class ManageCategoriesWindow : Window
    {
        private long _userId;

        public ManageCategoriesWindow(long userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadCategories();
        }

        private void LoadCategories()
        {
            using (var context = new AppDbContext())
            {
                // Загружаем только категории текущего пользователя
                var categories = context.Categories.Where(c => c.UserId == _userId).ToList();
                LstCategories.ItemsSource = categories;
                LstCategories.DisplayMemberPath = "Name"; // Отображаем название
            }
        }

        private void BtnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            string newCat = TxtNewCategory.Text.Trim();
            if (string.IsNullOrWhiteSpace(newCat)) return;

            try
            {
                using (var context = new AppDbContext())
                {
                    // Проверяем, есть ли уже такая категория у пользователя
                    if (context.Categories.Any(c => c.UserId == _userId && c.Name == newCat))
                    {
                        MessageBox.Show("Такая категория уже существует!", "Внимание");
                        return;
                    }

                    var category = new Category
                    {
                        Name = newCat,
                        UserId = _userId,
                        CreatedAt = DateTime.Now
                    };

                    context.Categories.Add(category);
                    context.SaveChanges();
                }

                TxtNewCategory.Clear();
                LoadCategories(); // Обновляем список
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }
    }
}
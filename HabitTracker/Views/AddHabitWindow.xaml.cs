using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HabitTracker.Data;
using HabitTracker.Models;

namespace HabitTracker.Views
{
    public partial class AddHabitWindow : Window
    {
        private long _userId;

        public string HabitName { get; private set; }
        public string CategoryName { get; private set; }
        public string Frequency { get; private set; }
        public string HabitType { get; private set; }
        public string Target { get; private set; }

        public AddHabitWindow(long userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadCategories();
        }

        private void LoadCategories()
        {
            using (var context = new AppDbContext())
            {
                var categories = context.Categories.Where(c => c.UserId == _userId).ToList();
                CmbCategory.ItemsSource = categories;
                CmbCategory.DisplayMemberPath = "Name";
                if (categories.Count > 0) CmbCategory.SelectedIndex = 0;
            }
        }

        private void BtnManageCategories_Click(object sender, RoutedEventArgs e)
        {
            // Передаем ID пользователя в менеджер категорий
            var manager = new ManageCategoriesWindow(_userId);
            manager.Owner = this;
            manager.ShowDialog();

            // После закрытия окна обновляем список категорий
            LoadCategories();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Введите название привычки!", "Внимание");
                return;
            }

            HabitName = TxtName.Text.Trim();

            // Достаем имя из выбранного объекта Category
            var selectedCat = CmbCategory.SelectedItem as Category;
            CategoryName = selectedCat?.Name ?? "Без категории";

            Frequency = (CmbFrequency.SelectedItem as ComboBoxItem)?.Content.ToString();
            HabitType = (CmbType.SelectedItem as ComboBoxItem)?.Content.ToString();
            Target = $"{TxtTarget.Text} {TxtUnit.Text}".Trim();

            this.DialogResult = true;
            this.Close();
        }
    }
}
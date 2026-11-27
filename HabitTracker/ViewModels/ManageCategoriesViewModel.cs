using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HabitTracker.Models;
using HabitTracker.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.ViewModels
{
    public partial class ManageCategoriesViewModel : ObservableObject
    {
        private readonly CategoryService _categoryService;
        private readonly long _userId;

        public ObservableCollection<Category> Categories { get; } = new();

        [ObservableProperty] private string _newCategoryName;
        [ObservableProperty] private Category _selectedCategory;
        [ObservableProperty] private string _newCategoryColor = "#28a745"; // По умолчанию зеленый

        public ManageCategoriesViewModel()
        {
            _categoryService = App.ServiceProvider.GetRequiredService<CategoryService>();
            _userId = AppState.CurrentUser.UserId;

            LoadCategories();
        }

        private void LoadCategories()
        {
            Categories.Clear();
            var cats = _categoryService.GetUserCategories(_userId);
            foreach (var c in cats) Categories.Add(c);
        }

        [RelayCommand]
        private void AddCategory()
        {
            if (string.IsNullOrWhiteSpace(NewCategoryName)) return;

            if (_categoryService.AddCategory(NewCategoryName, NewCategoryColor, _userId))
            {
                NewCategoryName = string.Empty;
                LoadCategories();
            }
            else
            {
                System.Windows.MessageBox.Show("Такая категория уже существует!");
            }
        }

        [RelayCommand]
        private void DeleteCategory()
        {
            if (SelectedCategory == null) return;

            if (_categoryService.DeleteCategory(SelectedCategory.CategoryId))
            {
                LoadCategories();
            }
            else
            {
                System.Windows.MessageBox.Show("Нельзя удалить категорию, к которой привязаны привычки!");
            }
        }
    }
}
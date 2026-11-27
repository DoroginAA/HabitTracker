using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HabitTracker.Factories;
using HabitTracker.Models;
using HabitTracker.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HabitTracker.ViewModels
{
    public partial class AddHabitViewModel : ObservableObject
    {
        private readonly HabitService _habitService;
        private readonly CategoryService _categoryService;
        private readonly IHabitFactory _habitFactory;
        private readonly long _userId;
        private readonly Habit? _habitToEdit;

        public ObservableCollection<Category> Categories { get; } = new();

        [ObservableProperty] private string _name;
        [ObservableProperty] private string _description;
        [ObservableProperty] private Category _selectedCategory;
        [ObservableProperty] private int _selectedFrequencyIndex = 0;
        [ObservableProperty] private int _selectedTypeIndex = 0;
        [ObservableProperty] private string _targetValue;
        [ObservableProperty] private string _targetUnit;
        [ObservableProperty] private DateTime? _startDate = DateTime.Now;
        [ObservableProperty] private DateTime? _endDate;

        public AddHabitViewModel()
        {
            _habitService = App.ServiceProvider.GetRequiredService<HabitService>();
            _categoryService = App.ServiceProvider.GetRequiredService<CategoryService>();

            LoadCategories();

            if (AppState.HabitToEdit != null) FillFields();
        }

        private void LoadCategories()
        {
            Categories.Clear();
            var cats = _categoryService.GetUserCategories(_userId);
            foreach (var c in cats) Categories.Add(c);

            if (Categories.Count > 0 && _habitToEdit == null) SelectedCategory = Categories[0];
        }

        private void FillFields()
        {
            Name = _habitToEdit.Name;
            Description = _habitToEdit.Description;
            TargetValue = _habitToEdit.TargetValue?.ToString();
            TargetUnit = _habitToEdit.TargetUnit;
            StartDate = _habitToEdit.StartDate;
            EndDate = _habitToEdit.EndDate;
            SelectedFrequencyIndex = _habitToEdit.FrequencyTypeId == 1 ? 0 : 1;
            SelectedTypeIndex = _habitToEdit.HabitTypeId == 1 ? 0 : 1;

            SelectedCategory = Categories.FirstOrDefault(c => c.CategoryId == _habitToEdit.CategoryId) ?? Categories.FirstOrDefault();
        }

        [RelayCommand]
        private void Save()
        {
            if (string.IsNullOrWhiteSpace(Name)) return;

            decimal? target = decimal.TryParse(TargetValue, out var val) ? val : null;

            if (_habitToEdit == null)
            {
                _habitService.CreateHabit(Name, _userId, SelectedCategory?.CategoryId,
                    SelectedFrequencyIndex + 1, SelectedTypeIndex + 1, Description, target, TargetUnit);
            }
            else
            {
                _habitToEdit.Name = Name;
                _habitToEdit.Description = Description;
                _habitToEdit.CategoryId = SelectedCategory?.CategoryId;
                _habitToEdit.FrequencyTypeId = SelectedFrequencyIndex + 1;
                _habitToEdit.HabitTypeId = SelectedTypeIndex + 1;
                _habitToEdit.TargetValue = target;
                _habitToEdit.TargetUnit = TargetUnit;
                _habitToEdit.StartDate = StartDate ?? DateTime.Now;
                _habitToEdit.EndDate = EndDate;
                _habitService.UpdateHabit(_habitToEdit); 
            }
        }
    }
}
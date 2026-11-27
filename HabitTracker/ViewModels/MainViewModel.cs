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
    public partial class MainViewModel : ObservableObject
    {
        private readonly HabitService _habitService;
        public User CurrentUser { get; }

        [ObservableProperty] private int _selectedTabIndex = 0;
        [ObservableProperty] private string _filter = "Все";

        public ObservableCollection<Habit> ActiveHabits { get; } = new();
        public ObservableCollection<Habit> ArchivedHabits { get; } = new();

        public MainViewModel()
        {
            _habitService = App.ServiceProvider.GetRequiredService<HabitService>();
            LoadHabits();
        }

        public void LoadHabits()
        {
            ActiveHabits.Clear();
            ArchivedHabits.Clear();

            var habits = _habitService.GetUserHabits(CurrentUser.UserId);

            foreach (var h in habits)
            {
                if (h.StatusId == 1) ActiveHabits.Add(h);
                else ArchivedHabits.Add(h);
            }
        }

        [RelayCommand] private void NavigateHabits() => SelectedTabIndex = 0;
        [RelayCommand] private void NavigateArchive() => SelectedTabIndex = 1;
        [RelayCommand] private void NavigateStats() => SelectedTabIndex = 2;
        [RelayCommand] private void NavigateSettings() => SelectedTabIndex = 3;
        [RelayCommand] private void NavigateAbout() => SelectedTabIndex = 4;
        [RelayCommand] private void NavigateProfile() => SelectedTabIndex = 5;

        [RelayCommand]
        private void ArchiveHabit(Habit habit)
        {
            _habitService.ArchiveHabit(habit);
            LoadHabits();
        }

        [RelayCommand]
        private void DeleteHabit(Habit habit)
        {
            _habitService.DeleteHabit(habit.HabitId);
            LoadHabits();
        }
    }
}

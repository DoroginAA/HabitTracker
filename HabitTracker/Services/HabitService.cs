using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitTracker.Factories;
using HabitTracker.Models;
using HabitTracker.Repositories;

namespace HabitTracker.Services
{
    public class HabitService
    {
        private readonly IHabitRepository _repository;
        private readonly IHabitFactory _factory;

        public HabitService(IHabitRepository repository, IHabitFactory factory)
        {
            _repository = repository;
            _factory = factory;
        }

        public IEnumerable<Habit> GetUserHabits(long userId)
        {
            return _repository.GetByUserId(userId);
        }

        public void CreateHabit(string name, long userId, long? categoryId, int frequency, int type, string? desc, decimal? target, string? unit)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название не может быть пустым");

            var habit = _factory.Create(name, userId, categoryId, frequency, type, desc, target, unit);
            _repository.Add(habit);
        }

        public void ArchiveHabit(Habit habit)
        {
            habit.StatusId = habit.StatusId == 1 ? 2 : 1; // Переключаем статус
            _repository.Update(habit);
        }

        public void DeleteHabit(long habitId)
        {
            _repository.Delete(habitId);
        }

        public void UpdateHabit(Habit habit)
        {
            if (habit == null)
                throw new ArgumentNullException(nameof(habit));

            _repository.Update(habit);
        }
    }
}
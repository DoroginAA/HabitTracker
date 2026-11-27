using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitTracker.Models;

namespace HabitTracker.Factories
{
    public class HabitFactory : IHabitFactory
    {
        public Habit Create(string name, long userId, long? categoryId, int frequencyTypeId, int habitTypeId, string? description, decimal? targetValue, string? targetUnit)
        {
            return new Habit
            {
                UserId = userId,
                Name = name,
                CategoryId = categoryId,
                FrequencyTypeId = frequencyTypeId,
                HabitTypeId = habitTypeId,
                Description = description,
                TargetValue = targetValue,
                TargetUnit = targetUnit,
                StatusId = 1, // 1 = Активна
                StartDate = DateTime.Now
            };
        }
    }
}
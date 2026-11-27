using HabitTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Factories
{
    public interface IHabitFactory
    {
        Habit Create(string name, long userId, long? categoryId, int frequencyTypeId, int habitTypeId, string? description, decimal? targetValue, string? targetUnit);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitTracker.Models;

namespace HabitTracker.Repositories
{
    public interface IHabitRepository
    {
        void Add(Habit habit);
        void Update(Habit habit);
        void Delete(long habitId);
        Habit? GetById(long habitId);
        IEnumerable<Habit> GetByUserId(long userId);
    }
}
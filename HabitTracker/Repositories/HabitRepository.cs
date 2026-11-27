using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitTracker.Data;
using HabitTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Repositories
{
    public class HabitRepository : IHabitRepository
    {
        public void Add(Habit habit)
        {
            using var context = new AppDbContext();
            context.Habits.Add(habit);
            context.SaveChanges();
        }

        public void Update(Habit habit)
        {
            using var context = new AppDbContext();
            context.Habits.Update(habit);
            context.SaveChanges();
        }

        public void Delete(long habitId)
        {
            using var context = new AppDbContext();
            var habit = context.Habits.Find(habitId);
            if (habit != null)
            {
                context.Habits.Remove(habit);
                context.SaveChanges();
            }
        }

        public Habit? GetById(long habitId)
        {
            using var context = new AppDbContext();
            return context.Habits.Find(habitId);
        }

        public IEnumerable<Habit> GetByUserId(long userId)
        {
            using var context = new AppDbContext();
            return context.Habits.Where(h => h.UserId == userId).Include(h => h.Category).ToList();
        }
    }
}
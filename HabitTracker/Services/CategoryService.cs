using HabitTracker.Data;
using HabitTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Services
{
    public class CategoryService
    {
        public List<Category> GetUserCategories(long userId)
        {
            using var context = new AppDbContext();
            return context.Categories.Where(c => c.UserId == userId).ToList();
        }

        public bool AddCategory(string name, string color, long userId)
        {
            using var context = new AppDbContext();
            if (context.Categories.Any(c => c.UserId == userId && c.Name == name)) return false;

            context.Categories.Add(new Category { Name = name, Color = color, UserId = userId });
            context.SaveChanges();
            return true;
        }

        public bool DeleteCategory(long categoryId)
        {
            using var context = new AppDbContext();
            if (context.Habits.Any(h => h.CategoryId == categoryId)) return false;

            var cat = context.Categories.Find(categoryId);
            if (cat != null)
            {
                context.Categories.Remove(cat);
                context.SaveChanges();
            }
            return true;
        }
    }
}
using HabitTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker
{
    public static class AppState
    {
        public static User? CurrentUser { get; set; }
        public static Habit? HabitToEdit { get; set; }
    }
}
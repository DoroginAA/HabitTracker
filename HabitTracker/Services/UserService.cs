using HabitTracker.Data;
using HabitTracker.Helpers;
using HabitTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Services
{
    public class UserService
    {
        public User? Login(string email, string password)
        {
            using var context = new AppDbContext();
            var user = context.Users.FirstOrDefault(u => u.Email == email);

            if (user != null && user.PasswordHash == PasswordHasher.Hash(password))
            {
                user.IsActive = true;
                user.LastLoginAt = DateTime.Now;
                context.SaveChanges();
                return user;
            }
            return null;
        }

        public bool Register(string username, string email, string password, out User? newUser)
        {
            newUser = null;
            using var context = new AppDbContext();

            if (context.Users.Any(u => u.Email == email)) return false;

            newUser = new User { Username = username, Email = email, PasswordHash = PasswordHasher.Hash(password) };
            context.Users.Add(newUser);
            context.SaveChanges();

            context.UserSettings.Add(new UserSetting { UserId = newUser.UserId, Theme = "Light" });
            context.SaveChanges();

            return true;
        }
    }
}
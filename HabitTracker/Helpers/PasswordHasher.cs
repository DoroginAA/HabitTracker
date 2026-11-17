using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace HabitTracker.Helpers
{
    /// <summary>
    /// Класс для хеширования паролей (SHA256).
    /// </summary>
    public static class PasswordHasher
    {
        /// <summary>
        /// Возвращает хеш пароля.
        /// </summary>
        public static string Hash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hashBytes = sha256.ComputeHash(bytes);

                // Преобразуем байты в строку
                var sb = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
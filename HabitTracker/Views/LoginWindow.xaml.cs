using HabitTracker.Data;
using HabitTracker.Models;
using HabitTracker.Helpers;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HabitTracker.Views
{
    public partial class LoginWindow : Window
    {
        public long LoggedInUserId { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
        }

        // Вход
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = TxtLoginEmail.Text.Trim();
            string password = TxtLoginPassword.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка");
                return;
            }

            try
            {
                string username = "";
                string useremail = "";
                using (var context = new AppDbContext())
                {
                    // Ищем пользователя
                    var user = context.Users.FirstOrDefault(u => u.Email == email);

                    // Проверяем, существует ли он, и совпадает ли ХЕШ его пароля с введенным
                    if (user == null || user.PasswordHash != PasswordHasher.Hash(password))
                    {
                        MessageBox.Show("Неверный Email или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    LoggedInUserId = user.UserId;
                    username = user.Username;
                    useremail = user.Email;
                }

                OpenMainWindow(username, useremail);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        // Регистрация
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = TxtRegUsername.Text.Trim();
            string email = TxtRegEmail.Text.Trim();
            string password = TxtRegPassword.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка");
                return;
            }

            try
            {
                using (var context = new AppDbContext())
                {
                    // Проверяем, занят ли email
                    if (context.Users.Any(u => u.Email == email))
                    {
                        MessageBox.Show("Пользователь с таким Email уже существует!", "Ошибка");
                        return;
                    }

                    // Создаем нового пользователя с ХЕШИРОВАННЫМ паролем
                    var newUser = new User
                    {
                        Username = username,
                        Email = email,
                        PasswordHash = PasswordHasher.Hash(password) // Хешируем!
                    };
                    context.Users.Add(newUser);
                    context.SaveChanges();

                    MessageBox.Show("Регистрация успешна! Теперь вы можете войти.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Очищаем поля и перекидываем на вкладку входа
                    TxtRegUsername.Text = ""; TxtRegEmail.Text = ""; TxtRegPassword.Password = "";
                    TxtLoginEmail.Text = email; TxtLoginPassword.Focus();
                    LoggedInUserId = newUser.UserId;
                    OpenMainWindow(newUser.Username, newUser.Email);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void OpenMainWindow(string username, string email)
        {
            var mainWindow = new MainWindow(LoggedInUserId, username, email);
            mainWindow.Show();
            this.Close();
        }
    }
}
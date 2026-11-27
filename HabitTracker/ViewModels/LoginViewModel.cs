using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HabitTracker.Models;
using HabitTracker.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly UserService _userService;
        private readonly Action<User> _onLoginSuccess; // Делегат для перехода на главное окно

        [ObservableProperty] private string _loginEmail;
        [ObservableProperty] private string _loginPassword;

        [ObservableProperty] private string _regUsername;
        [ObservableProperty] private string _regEmail;
        [ObservableProperty] private string _regPassword;

        public LoginViewModel()
        {
            _userService = App.ServiceProvider.GetRequiredService<UserService>();
        }

        [RelayCommand]
        private void Login()
        {
            if (string.IsNullOrWhiteSpace(LoginEmail) || string.IsNullOrWhiteSpace(LoginPassword)) return;

            var user = _userService.Login(LoginEmail, LoginPassword);
            if (user != null)
                _onLoginSuccess(user);
            else
                System.Windows.MessageBox.Show("Неверный Email или пароль!");
        }

        [RelayCommand]
        private void Register()
        {
            if (string.IsNullOrWhiteSpace(RegUsername) || string.IsNullOrWhiteSpace(RegEmail) || string.IsNullOrWhiteSpace(RegPassword)) return;

            if (_userService.Register(RegUsername, RegEmail, RegPassword, out var newUser))
            {
                System.Windows.MessageBox.Show("Регистрация успешна!");
                _onLoginSuccess(newUser!);
            }
            else
            {
                System.Windows.MessageBox.Show("Пользователь с таким Email уже существует!");
            }
        }
    }
}
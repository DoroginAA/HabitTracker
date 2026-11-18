using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using HabitTracker.Data;
using HabitTracker.Models;

namespace HabitTracker
{
    public partial class MainWindow : Window
    {
        private long _currentUserId;
        private string _currentUsername;
        private string _currentEmail;
        private bool _isDarkTheme = false;

        // Списки для хранения карточек (активные и архивные)
        private List<Border> _activeHabitCards = new List<Border>();
        private List<Border> _archivedHabitCards = new List<Border>();

        public MainWindow(long userId, string username, string email)
        {
            InitializeComponent();
            _currentUserId = userId;
            _currentUsername = username;
            _currentEmail = email;

            TxtUsername.Text = _currentUsername;
            TxtProfileName.Text = _currentUsername;
            TxtProfileEmail.Text = $"Email: {_currentEmail}";

            LoadMockHabits();
        }

        // Навигация
        private void MenuHabits_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedIndex = 0;
        private void MenuStats_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedIndex = 1;
        private void MenuSettings_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedIndex = 2;
        private void MenuArchive_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedIndex = 3;
        private void MenuAbout_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedIndex = 4;
        private void BtnProfile_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedIndex = 5;

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var login = new Views.LoginWindow();
            login.Show();
            this.Close();
        }

        // Окно добавления
        private void BtnOpenAddWindow_Click(object sender, RoutedEventArgs e)
        {
            // Передаем ID пользователя для загрузки его категорий из БД
            var addWindow = new Views.AddHabitWindow(_currentUserId);
            addWindow.Owner = this;

            if (addWindow.ShowDialog() == true)
            {
                CreateHabitCard(
                    addWindow.HabitName,
                    addWindow.CategoryName,
                    addWindow.Frequency,
                    addWindow.HabitType,
                    addWindow.Target
                );
            }
        }

        // Аватарка
        private void BtnChangeAvatar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                    ImgProfileAvatar.Source = bitmap;
                    ImgAvatar.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки: {ex.Message}");
                }
            }
        }

        // Экспорт (пока заглушка)
        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
                MessageBox.Show($"Данные успешно экспортированы в формат {btn.Tag} (заглушка)", "Экспорт");
        }

        // Тёмная тема меню
        private void ChkDarkTheme_Click(object sender, RoutedEventArgs e)
        {
            _isDarkTheme = ChkDarkTheme.IsChecked ?? false;

            Brush bgDark = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            Brush panelDark = new SolidColorBrush(Color.FromRgb(45, 45, 45));
            Brush textDark = Brushes.White;
            Brush textLight = Brushes.Black;
            Brush panelLight = Brushes.White;

            if (_isDarkTheme)
            {
                this.Background = bgDark;
                LeftSidebar.Background = new SolidColorBrush(Color.FromRgb(20, 20, 20));
                MainTabControl.Background = bgDark;

                // Панели
                TopControlPanel.Background = panelDark;
                ArchiveHeader.Background = panelDark;
                SettingsBorder.Background = panelDark;
                AboutBorder.Background = panelDark;
                ProfileBorder.Background = panelDark;
                StatBorder1.Background = panelDark;
                StatBorder2.Background = panelDark;
                StatBorder3.Background = panelDark;
                ChartBorder.Background = panelDark;

                // Текст
                TxtProfileHeader.Foreground = textDark;
                TxtProfileName.Foreground = textDark;
                TxtUsername.Foreground = textDark;

                foreach (var child in ((Grid)LeftSidebar.Child).Children)
                {
                    if (child is StackPanel sp)
                        foreach (var item in sp.Children)
                            if (item is Button btn && btn.Style == FindResource("MenuButton")) btn.Foreground = Brushes.LightGray;
                }

                UpdateAllCardsColor(panelDark, textDark, Brushes.LightGray);
            }
            else
            {
                this.Background = new SolidColorBrush(Color.FromRgb(244, 245, 247));
                LeftSidebar.Background = panelLight;
                MainTabControl.Background = Brushes.Transparent;

                // Панели
                TopControlPanel.Background = panelLight;
                ArchiveHeader.Background = panelLight;
                SettingsBorder.Background = panelLight;
                AboutBorder.Background = panelLight;
                ProfileBorder.Background = panelLight;
                StatBorder1.Background = panelLight;
                StatBorder2.Background = panelLight;
                StatBorder3.Background = panelLight;
                ChartBorder.Background = panelLight;

                // Текст
                TxtProfileHeader.Foreground = textLight;
                TxtProfileName.Foreground = textLight;
                TxtUsername.Foreground = textLight;

                foreach (var child in ((Grid)LeftSidebar.Child).Children)
                {
                    if (child is StackPanel sp)
                        foreach (var item in sp.Children)
                            if (item is Button btn && btn.Style == FindResource("MenuButton")) btn.Foreground = new SolidColorBrush(Color.FromRgb(85, 85, 85));
                }

                UpdateAllCardsColor(panelLight, new SolidColorBrush(Color.FromRgb(51, 51, 51)), Brushes.Gray);
            }
        }

        private void UpdateAllCardsColor(Brush cardBg, Brush titleFg, Brush infoFg)
        {
            foreach (var card in _activeHabitCards.Concat(_archivedHabitCards))
            {
                card.Background = cardBg;
                if (card.Child is Grid g && g.Children[0] is StackPanel leftP && leftP.Children[1] is StackPanel textP)
                {
                    if (textP.Children[0] is TextBlock title) title.Foreground = titleFg;
                    if (textP.Children[1] is TextBlock info) info.Foreground = infoFg;
                }
            }
        }

        // Фильтрация
        private void CmbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HabitsListPanel == null || CmbFilter.SelectedItem == null) return;
            string filter = (CmbFilter.SelectedItem as ComboBoxItem).Content.ToString();
            HabitsListPanel.Children.Clear();

            foreach (var card in _activeHabitCards)
            {
                string type = card.Tag?.ToString() ?? "";
                if (filter == "Все" || type == filter) HabitsListPanel.Children.Add(card);
            }
        }

        // Создание привычки
        private void CreateHabitCard(string habitName, string category, string frequency, string habitType, string target)
        {
            Border card = new Border
            {
                Background = _isDarkTheme ? new SolidColorBrush(Color.FromRgb(45, 45, 45)) : Brushes.White,
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(15),
                Margin = new Thickness(0, 0, 0, 10),
                Tag = habitType
            };

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Левая часть
            StackPanel textPanel = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            TextBlock titleTb = new TextBlock
            {
                Text = habitName,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = _isDarkTheme ? Brushes.White : new SolidColorBrush(Color.FromRgb(51, 51, 51))
            };
            textPanel.Children.Add(titleTb);

            TextBlock infoTb = new TextBlock
            {
                Text = $"📁 {category} | 🎯 {target} | 🔄 {frequency} | {(habitType == "Вредная" ? "❌ Вредная" : "✅ Полезная")}",
                FontSize = 12,
                Foreground = Brushes.Gray,
                Margin = new Thickness(0, 5, 0, 0)
            };
            textPanel.Children.Add(infoTb);
            Grid.SetColumn(textPanel, 0);
            grid.Children.Add(textPanel);

            // Правая часть (Кнопки)
            StackPanel buttonsPanel = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };

            // Кнопка Выполнить
            Button completeBtn = new Button
            {
                Content = "Выполнить",
                Background = new SolidColorBrush(Color.FromRgb(40, 167, 69)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(15, 8, 15, 8),
                Margin = new Thickness(0, 0, 10, 0)
            };
            bool isCompleted = false;
            completeBtn.Click += (s, ev) =>
            {
                isCompleted = !isCompleted;
                if (isCompleted)
                {
                    titleTb.TextDecorations = TextDecorations.Strikethrough;
                    titleTb.Opacity = 0.5;
                    completeBtn.Content = "Отменить";
                    completeBtn.Background = Brushes.Gray;
                }
                else
                {
                    titleTb.TextDecorations = null;
                    titleTb.Opacity = 1.0;
                    completeBtn.Content = "Выполнить";
                    completeBtn.Background = new SolidColorBrush(Color.FromRgb(40, 167, 69));
                }
            };

            // Кнопка В архив
            Button archiveBtn = new Button
            {
                Content = "📦 В архив",
                Background = new SolidColorBrush(Color.FromRgb(255, 193, 7)),
                Foreground = Brushes.Black,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(15, 8, 15, 8),
                Margin = new Thickness(0, 0, 10, 0)
            };
            archiveBtn.Click += (s, ev) =>
            {
                HabitsListPanel.Children.Remove(card);
                _activeHabitCards.Remove(card);

                archiveBtn.Content = "🔄 Восстановить";
                archiveBtn.Background = new SolidColorBrush(Color.FromRgb(23, 162, 184));

                archiveBtn.Click += (sender2, args2) =>
                {
                    ArchivedListPanel.Children.Remove(card);
                    _archivedHabitCards.Remove(card);
                    HabitsListPanel.Children.Add(card);
                    _activeHabitCards.Add(card);
                    archiveBtn.Content = "📦 В архив";
                    archiveBtn.Background = new SolidColorBrush(Color.FromRgb(255, 193, 7));
                };

                ArchivedListPanel.Children.Add(card);
                _archivedHabitCards.Add(card);
            };

            // Кнопка Удалить
            Button deleteBtn = new Button
            {
                Content = "Удалить",
                Background = new SolidColorBrush(Color.FromRgb(220, 53, 69)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(15, 8, 15, 8)
            };
            deleteBtn.Click += (s, ev) =>
            {
                HabitsListPanel.Children.Remove(card);
                ArchivedListPanel.Children.Remove(card);
                _activeHabitCards.Remove(card);
                _archivedHabitCards.Remove(card);
            };

            buttonsPanel.Children.Add(archiveBtn);
            buttonsPanel.Children.Add(completeBtn);
            buttonsPanel.Children.Add(deleteBtn);

            Grid.SetColumn(buttonsPanel, 1);
            grid.Children.Add(buttonsPanel);

            card.Child = grid;
            _activeHabitCards.Add(card);
            HabitsListPanel.Children.Add(card);
        }

        private void LoadMockHabits()
        {
            CreateHabitCard("Выпить стакан воды", "Здоровье", "Ежедневно", "Полезная", "2 литра");
            CreateHabitCard("Читать книгу", "Обучение", "Ежедневно", "Полезная", "30 минут");
            CreateHabitCard("Сходить в спортзал", "Спорт", "Еженедельно", "Полезная", "3 раза");
            CreateHabitCard("Съесть фастфуд", "Здоровье", "Ежедневно", "Вредная", "0 раз");
        }
    }
}
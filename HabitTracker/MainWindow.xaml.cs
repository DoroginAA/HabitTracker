using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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

        private UserSetting _currentSettings;

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

            using (var context = new AppDbContext())
            {
                _currentSettings = context.UserSettings.FirstOrDefault(s => s.UserId == _currentUserId);

                // Если настроек нет, создаем их
                if (_currentSettings == null)
                {
                    _currentSettings = new UserSetting { UserId = _currentUserId, Theme = "Light" };
                    context.UserSettings.Add(_currentSettings);
                    context.SaveChanges();
                }
            }

            if (!string.IsNullOrEmpty(_currentSettings.AvatarPath))
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _currentSettings.AvatarPath);

                if (File.Exists(fullPath))
                {
                    try
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.UriSource = new Uri(fullPath);
                        bitmap.EndInit();
                        bitmap.Freeze();

                        ImgAvatar.Source = bitmap;
                        ImgProfileAvatar.Source = bitmap;
                    }
                    catch {  }
                }
            }

            LoadMockHabits();

            bool isDark = _currentSettings.Theme == "Dark";
            ChkDarkTheme.IsChecked = isDark;
            ApplyTheme(isDark);
        }

        private void ApplyTheme(bool isDark)
        {
            _isDarkTheme = isDark;

            Brush bgDark = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            Brush panelDark = new SolidColorBrush(Color.FromRgb(45, 45, 45));
            Brush textDark = Brushes.White;
            Brush textLight = Brushes.Black;
            Brush panelLight = Brushes.White;

            if (isDark)
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

                // Текст профиля и меню
                TxtProfileHeader.Foreground = textDark;
                TxtProfileName.Foreground = textDark;
                TxtUsername.Foreground = textDark;

                // Текст фильтра
                TxtFilterLabel.Foreground = textDark;
                CmbFilter.Foreground = textDark;
                CmbFilter.Background = panelDark;

                // Текст в Настройках и О программе (ИСПРАВЛЕНИЕ)
                TxtSettingsTitle.Foreground = textDark;
                ChkReminders.Foreground = textDark;
                ChkDarkTheme.Foreground = textDark;
                TxtExportTitle.Foreground = textDark;

                TxtAboutTitle.Foreground = textDark;
                TxtAboutDesc.Foreground = textDark;
                TxtAboutAuthor.Foreground = textDark;

                TxtArchiveHeader.Foreground = textDark;

                // Меню
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

                TxtFilterLabel.Foreground = textLight;
                CmbFilter.Foreground = textLight;
                CmbFilter.Background = Brushes.White;

                // Текст в Настройках и "О программе"
                TxtSettingsTitle.Foreground = textLight;
                ChkReminders.Foreground = textLight;
                ChkDarkTheme.Foreground = textLight;
                TxtExportTitle.Foreground = textLight;

                TxtAboutTitle.Foreground = textLight;
                TxtAboutDesc.Foreground = textLight;
                TxtAboutAuthor.Foreground = textLight;

                TxtArchiveHeader.Foreground = textLight;

                foreach (var child in ((Grid)LeftSidebar.Child).Children)
                {
                    if (child is StackPanel sp)
                        foreach (var item in sp.Children)
                            if (item is Button btn && btn.Style == FindResource("MenuButton")) btn.Foreground = new SolidColorBrush(Color.FromRgb(85, 85, 85));
                }

                UpdateAllCardsColor(panelLight, new SolidColorBrush(Color.FromRgb(51, 51, 51)), Brushes.Gray);
            }
        }

        // Метод покраски карточек
        private void UpdateAllCardsColor(Brush cardBg, Brush titleFg, Brush infoFg)
        {
            var allCards = _activeHabitCards.Concat(_archivedHabitCards);
            foreach (var card in allCards)
            {
                card.Background = cardBg;

                if (card.Child is Grid g)
                {
                    foreach (var child in g.Children)
                    {
                        if (child is StackPanel textPanel)
                        {
                            foreach (var textChild in textPanel.Children)
                            {
                                if (textChild is TextBlock tb)
                                {
                                    if (tb.FontSize == 18) tb.Foreground = titleFg;
                                    else if (tb.FontSize == 12) tb.Foreground = infoFg;
                                }
                            }
                        }
                    }
                }
            }
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
                    string sourceFile = openFileDialog.FileName;
                    string extension = Path.GetExtension(sourceFile);

                    string avatarsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatars");
                    if (!Directory.Exists(avatarsFolder))
                        Directory.CreateDirectory(avatarsFolder);

                    string newFileName = $"user_{_currentUserId}{extension}";
                    string relativePath = Path.Combine("Avatars", newFileName);
                    string fullDestPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

                    // Отвязываем текущие картинки от интерфейса, чтобы файл освободился
                    ImgProfileAvatar.Source = null;
                    ImgAvatar.Source = null;

                    // Удаляем старую аватарку
                    if (!string.IsNullOrEmpty(_currentSettings.AvatarPath))
                    {
                        string oldFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _currentSettings.AvatarPath);
                        if (File.Exists(oldFullPath))
                        {
                            File.Delete(oldFullPath);
                        }
                    }

                    File.Copy(sourceFile, fullDestPath, true);

                    // Загружаем картинку через FileStream
                    BitmapImage bitmap = new BitmapImage();
                    using (FileStream stream = new FileStream(fullDestPath, FileMode.Open, FileAccess.Read))
                    {
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                    }
                    bitmap.Freeze();

                    // Устанавливаем новую картинку в интерфейс
                    ImgProfileAvatar.Source = bitmap;
                    ImgAvatar.Source = bitmap;

                    // Сохраняем в БД
                    using (var context = new AppDbContext())
                    {
                        var settings = context.UserSettings.FirstOrDefault(s => s.UserId == _currentUserId);
                        if (settings != null)
                        {
                            settings.AvatarPath = relativePath;
                            context.SaveChanges();
                        }
                    }
                    _currentSettings.AvatarPath = relativePath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении аватара: {ex.Message}");
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
            bool isDark = ChkDarkTheme.IsChecked ?? false;

            // Применение цветов
            ApplyTheme(isDark);

            // Сохранение в БД
            using (var context = new AppDbContext())
            {
                var settings = context.UserSettings.FirstOrDefault(s => s.UserId == _currentUserId);
                if (settings != null)
                {
                    settings.Theme = isDark ? "Dark" : "Light";
                    context.SaveChanges();
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

            bool isArchived = false;

            archiveBtn.Click += (s, ev) =>
            {
                if (!isArchived)
                {
                    // ПЕРЕМЕЩАЕМ В АРХИВ
                    HabitsListPanel.Children.Remove(card);
                    _activeHabitCards.Remove(card);

                    ArchivedListPanel.Children.Add(card);
                    _archivedHabitCards.Add(card);

                    archiveBtn.Content = "🔄 Восстановить";
                    archiveBtn.Background = new SolidColorBrush(Color.FromRgb(23, 162, 184)); // Голубой
                    isArchived = true;
                }
                else
                {
                    // ВОССТАНАВЛИВАЕМ ИЗ АРХИВА
                    ArchivedListPanel.Children.Remove(card);
                    _archivedHabitCards.Remove(card);

                    HabitsListPanel.Children.Add(card);
                    _activeHabitCards.Add(card);

                    archiveBtn.Content = "📦 В архив";
                    archiveBtn.Background = new SolidColorBrush(Color.FromRgb(255, 193, 7)); // Желтый
                    isArchived = false;
                }
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
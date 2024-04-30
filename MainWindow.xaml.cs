using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace KALENDAR
{
    public partial class MainWindow : Window
    {
        private DateTime currentDate;

        public MainWindow()
        {
            InitializeComponent();
            currentDate = DateTime.Today;
            UpdateCalendar();
        }
        private void CreateDayButton(int day, int column, int row)
        {
            Button dayButton = new Button() { Content = day.ToString(), Margin = new Thickness(5) };
            Grid.SetColumn(dayButton, column);
            Grid.SetRow(dayButton, row);
            dayButton.Click += DayButton_Click;
            dayButton.ContextMenu = (ContextMenu)FindResource("DayContextMenu");
            dayButton.Tag = new DateTime(currentDate.Year, currentDate.Month, day);
            calendarGrid.Children.Add(dayButton);
        }

        private void UpdateCalendar()
        {
            calendarGrid.Children.Clear();
            calendarGrid.RowDefinitions.Clear();

            int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
            DayOfWeek firstDayOfWeek = new DateTime(currentDate.Year, currentDate.Month, 1).DayOfWeek;

            for (int i = 0; i < 7; i++)
            {
                calendarGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }

            string[] dayNames = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };
            for (int i = 0; i < dayNames.Length; i++)
            {
                Button dayButton = new Button() { Content = dayNames[i], IsEnabled = false, Background = Brushes.LightGray };
                Grid.SetColumn(dayButton, (int)firstDayOfWeek + i >= 7 ? (int)firstDayOfWeek + i - 7 : (int)firstDayOfWeek + i);
                calendarGrid.Children.Add(dayButton);
            }

            for (int i = 1; i <= daysInMonth; i++)
            {
                int column = (int)(new DateTime(currentDate.Year, currentDate.Month, i).DayOfWeek);
                int row = (i - 1 + (int)firstDayOfWeek) / 7 + 1;
                if (row > calendarGrid.RowDefinitions.Count)
                {
                    calendarGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                }
                CreateDayButton(i, column, row);
            }

            txtMonthYear.Text = currentDate.ToString("MMMM yyyy");
        }

        private void DayButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            DateTime selectedDate = (DateTime)clickedButton.Tag; 

            if (e.OriginalSource is ContextMenu)
            {
                ContextMenu contextMenu = (ContextMenu)clickedButton.ContextMenu;
                contextMenu.PlacementTarget = clickedButton;
                contextMenu.IsOpen = true;
            }
            else
            {
                OpenDay_Click(sender, e);
            }
        }

        private void OpenDay_Click(object sender, RoutedEventArgs e)
        {
            Button dayButton = (Button)sender;
            DateTime selectedDate = (DateTime)dayButton.Tag; 

            try
            {
                ActivitySelectionWindow activityWindow = new ActivitySelectionWindow(selectedDate);
                activityWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при открытии окна ActivitySelectionWindow: " + ex.Message);
            }
        }

        private void ClearDay_Click(object sender, RoutedEventArgs e)
        {
            Button dayButton = (Button)sender;
            DateTime selectedDate = (DateTime)dayButton.Tag;

            string jsonFilePath = "activities.json";

            if (File.Exists(jsonFilePath))
            {
                string jsonString = File.ReadAllText(jsonFilePath);

                List<Activity> activities = JsonConvert.DeserializeObject<List<Activity>>(jsonString);

                activities.RemoveAll(activity => activity.Date.Date == selectedDate.Date);

                string updatedJsonString = JsonConvert.SerializeObject(activities, Formatting.Indented);

                File.WriteAllText(jsonFilePath, updatedJsonString);
            }
            else
            {
                MessageBox.Show("Файл activities.json не найден.");
            }
        }

        private void NavigateBack_Click(object sender, RoutedEventArgs e)
        {
            currentDate = currentDate.AddMonths(-1);
            UpdateCalendar();
        }

        private void NavigateForward_Click(object sender, RoutedEventArgs e)
        {
            currentDate = currentDate.AddMonths(1);
            UpdateCalendar();
        }
    }


    public class Activity
    {
        public DateTime Date { get; set; }
        
    }
}
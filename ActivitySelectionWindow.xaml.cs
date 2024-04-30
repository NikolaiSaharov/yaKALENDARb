using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace KALENDAR
{
    public partial class ActivitySelectionWindow : Window
    {
        private DateTime selectedDate;
        private Dictionary<DateTime, List<string>> activities;
        private const string FileName = "activities.json";

        public ActivitySelectionWindow(DateTime selectedDate)
        {
            InitializeComponent();
            this.selectedDate = selectedDate;
            activities = LoadActivitiesFromFile();

            List<string> selectedActivities = GetSelectedActivitiesForDate(selectedDate);
            LoadActivities(selectedActivities);
        }

        private void LoadActivities(List<string> selectedActivities)
        {
            
            stackPanelActivities.Children.Clear();

            
            List<string> activityList = new List<string> { "CS2", "Dota2", "Palworld", "Minecraft", "Весёлая  ферма", "DayZ" }; 
            foreach (string activity in activityList)
            {
                CheckBox checkBox = new CheckBox { Content = activity, IsChecked = selectedActivities.Contains(activity) };
                stackPanelActivities.Children.Add(checkBox);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedActivities = new List<string>();
            foreach (CheckBox checkBox in stackPanelActivities.Children.OfType<CheckBox>())
            {
                if (checkBox.IsChecked == true)
                {
                    selectedActivities.Add(checkBox.Content.ToString());
                }
            }
            activities[selectedDate] = selectedActivities;
            SaveActivitiesToFile();
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private Dictionary<DateTime, List<string>> LoadActivitiesFromFile()
        {
            if (File.Exists(FileName))
            {
                string json = File.ReadAllText(FileName);
                return JsonConvert.DeserializeObject<Dictionary<DateTime, List<string>>>(json) ?? new Dictionary<DateTime, List<string>>();
            }
            return new Dictionary<DateTime, List<string>>();
        }

        private void SaveActivitiesToFile()
        {
            string json = JsonConvert.SerializeObject(activities, Formatting.Indented);
            File.WriteAllText(FileName, json);
        }

        private List<string> GetSelectedActivitiesForDate(DateTime date)
        {
            if (activities.TryGetValue(date, out List<string> selectedActivities))
            {
                return selectedActivities;
            }
            return new List<string>();
        }

    }
}
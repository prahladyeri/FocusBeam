/**
 * AppSettings.cs
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using Newtonsoft.Json;
using System.IO;

namespace focusbeam.Helpers
{
    public class AppSettings
    {
        private static string _filePath;
        private static string _salt = "fUvcePiyrdLkj";

        public bool ShowPomodoroAlerts { get; set; } = true;
        public int PomodoroInterval { get; set; } = 25; // minutes
        public bool AutoStartNextSession { get; set; } = false;
        public int PomodoroShortBreakInterval { get; set; } = 5; // minutes
        public int PomodoroLongBreakFrequency { get; set; } = 4; // pomodoros
        public int PomodoroLongBreakInterval { get; set; } = 15; // minutes
        public bool ShowMotivationTipsOnBreaks { get; set; } = false;
        public bool ShowWaterRemindersOnBreaks { get; set; } = true;

        //public bool EnableIdleDetection { get; set; } = true;
        //public int IdleTimeoutMinutes { get; set; } = 10;

        public static AppSettings Load(string secretKey = null, string filepath = "settings.json")
        {
            _filePath = filepath;
            if (!File.Exists(_filePath))
                return new AppSettings();

            string content = File.ReadAllText(_filePath);
            if (!string.IsNullOrEmpty(secretKey))
                content = StringHelper.Decrypt(content, secretKey, _salt);
            return JsonConvert.DeserializeObject<AppSettings>(content);
        }

        public static void Save(AppSettings settings, string secretKey = null)
        {
            //TODO: Add validate section or method
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            if (!string.IsNullOrEmpty(secretKey))
                json = StringHelper.Encrypt(json, secretKey, _salt);
            File.WriteAllText(_filePath, json);
        }
    }
}

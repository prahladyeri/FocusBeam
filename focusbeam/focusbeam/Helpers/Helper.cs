/**
 * Helper.cs
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace focusbeam.Helpers
{

    internal static class EmojiConstants
    {
        public const string PLUS = "➕";
        public const string PENCIL = "✏️";
        public const string PEN = "🖊";
        public const string TOOLS = "🛠";
        public const string BRAIN = "🧠";
        public const string WEB = "🌐"; // Suggests web-like structure
        public const string QUESTION = "❓";


        // Time / Scheduling
        public const string CLOCK_ANALOG = "🕒";
        public const string CALENDAR = "📅";
        public const string ALARM_CLOCK = "⏰";
        public const string HOURGLASS = "⏳";

        // Project and Task
        public const string PROJECT = "📁";
        public const string TASK = "📝";
        public const string CHECKLIST = "✅";
        public const string BULLET = "•";  // For lists

        // Status and Progress
        public const string DONE = "✔️";
        public const string PENDING = "🕐";
        public const string ERROR = "❌";
        public const string WARNING = "⚠️";
        public const string INFO = "ℹ️";
        public const string IN_PROGRESS = "🔄";

        // UI / Actions
        public const string START = "▶️";
        public const string STOP = "⏹️";
        public const string PAUSE = "⏸️";
        public const string REFRESH = "🔄";
        public const string SAVE = "💾";
        public const string SETTINGS = "⚙️";

        // Communication
        public const string COMMENT = "💬";
        public const string NOTIFICATION = "🔔";
        public const string ATTACHMENT = "📎";

        // Misc / Flair
        public const string ROCKET = "🚀";
        public const string STAR = "⭐";
        public const string FIRE = "🔥";
        public const string LIGHTBULB = "💡";
        public const string LOCK = "🔒";
        public const string UNLOCK = "🔓";
    }

    public static class AssemblyInfoHelper
    {
        public static string Title
        {
            get
            {
                var attr = Assembly.GetExecutingAssembly()
                    .GetCustomAttribute<AssemblyTitleAttribute>();
                return attr?.Title ?? "Untitled";
            }
        }
        public static string Copyright
        {
            get
            {
                return Assembly.GetExecutingAssembly()
                    .GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
            }
        }

        public static string Company
        {
            get
            {
                return Assembly.GetExecutingAssembly()
                    .GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            }
        }

        public static string GetDescription()
        {
            var attr = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyDescriptionAttribute>();
            return attr?.Description ?? "No description available.";
        }

        public static string GetVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            return version != null ? $"{version.Major}.{version.Minor}" : "0.0";
        }


    }




    internal static class Helper
    {


        internal static bool IsNumericType(Type type)
        {
            return type == typeof(byte) ||
                   type == typeof(sbyte) ||
                   type == typeof(short) ||
                   type == typeof(ushort) ||
                   type == typeof(int) ||
                   type == typeof(uint) ||
                   type == typeof(long) ||
                   type == typeof(ulong) ||
                   type == typeof(float) ||
                   type == typeof(double) ||
                   type == typeof(decimal);
        }

        public static T DeepClone<T>(T obj)
        {
            return JsonConvert.DeserializeObject<T>(
                JsonConvert.SerializeObject(obj));
        }
    }
}

﻿/**
 * Helpers.cs - Utility hub for general use
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace focusbeam.Util
{
    public enum SystemSoundTheme
    {
        Asterisk,
        Beep,
        Exclamation,
        Hand,
        Question
    }

    public static class SoundPlayerHelper
    {
        public static void Play(SystemSoundTheme theme)
        {
            switch (theme)
            {
                case SystemSoundTheme.Asterisk:
                    System.Media.SystemSounds.Asterisk.Play();
                    break;
                case SystemSoundTheme.Beep:
                    System.Media.SystemSounds.Beep.Play();
                    break;
                case SystemSoundTheme.Exclamation:
                    System.Media.SystemSounds.Exclamation.Play();
                    break;
                case SystemSoundTheme.Hand:
                    System.Media.SystemSounds.Hand.Play();
                    break;
                case SystemSoundTheme.Question:
                    System.Media.SystemSounds.Question.Play();
                    break;
            }
        }
    }



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


    internal static class FormHelper {

        private static ToolTip sharedToolTip = new ToolTip
        {
            AutoPopDelay = 5000,
            InitialDelay = 200,
            ReshowDelay = 500,
            ShowAlways = true
        };

        internal static string RecordSaveMessage(object obj) {
            return $"{obj.GetType().Name} saved.";
        }


        internal static void CreateTooltip(Control ctrl, string text)
        {
            sharedToolTip.SetToolTip(ctrl, text);
        }

        internal static void SetFocusToFirstEditableControl(TableLayoutPanel tlp)
        {
            // Get all controls on the form and order them by TabIndex
            // Filtering for controls that are visible, enabled, and can receive focus.
            var editableControls = tlp.Controls.Cast<Control>()
                .Where(c => c.Visible && c.Enabled && c.CanSelect)
                .OrderBy(c => c.TabIndex) // Order by TabIndex for logical flow
                .ToList(); // Convert to list to iterate

            foreach (Control control in editableControls)
            {
                // Check common editable control types
                if (control is TextBox ||
                    control is ComboBox ||
                    control is NumericUpDown ||
                    control is DateTimePicker ||
                    control is MaskedTextBox)
                {
                    control.Select(); // Give focus to this control
                    return; // Exit after finding and focusing the first one
                }
                // If the control is a container (like GroupBox or Panel),
                // you might want to recursively check its children.
                // For simplicity, this example only checks top-level controls.
                // If you need recursive checking, it gets more complex.
            }

            // Fallback: If no specific editable control is found, try to focus the first selectable one.
            var firstSelectableControl = tlp.Controls.Cast<Control>()
                .Where(c => c.Visible && c.Enabled && c.CanSelect)
                .OrderBy(c => c.TabIndex)
                .FirstOrDefault();

            firstSelectableControl?.Select();
        }
    }

    internal static class CryptoHelper {
        internal static string Encrypt(string plainText, string password, string salt)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
                var key = new Rfc2898DeriveBytes(password, saltBytes, 10000);
                aes.Key = key.GetBytes(32);
                aes.IV = key.GetBytes(16);
                
                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream()) 
                { 
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                        sw.Write(plainText);
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        internal static string Decrypt(string cipherText, string password, string salt)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
                var key = new Rfc2898DeriveBytes(password, saltBytes, 10000);
                aes.Key = key.GetBytes(32);
                aes.IV = key.GetBytes(16);

                byte[] buffer = Convert.FromBase64String(cipherText);

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream(buffer))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs)) {
                    return sr.ReadToEnd();
                }
            }
        }
    }

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
                content = CryptoHelper.Decrypt(content, secretKey, _salt);
            return JsonConvert.DeserializeObject<AppSettings>(content);
        }

        public static void Save(AppSettings settings, string secretKey = null)
        {
            //TODO: Add validate section or method
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            if (!string.IsNullOrEmpty(secretKey))
                json = CryptoHelper.Encrypt(json, secretKey, _salt);
            File.WriteAllText(_filePath, json);
        }
    }

    public static class Mailer
    {

        public static void SendEmail(string[] to, string subject, string body, string[] attachments = null)
        {
            attachments = attachments ?? new string[0];
            string htmlBody = Util.FileHelper.ReadEmbeddedResource("focusbeam.files.email.html")
                .Replace("{{body}}", body)
                .Replace("{{subject}}", subject)
                .Replace("{{regards}}", $"{Application.ProductName} Support")
                ;
            var fromAddress = new MailAddress("support@example.com", $"{Application.ProductName} Support");

            var smtp = new SmtpClient
            {
                Host = "mail.outlook.com",  // e.g., smtp.gmail.com
                Port = 587,                      // or 465 for SSL, or 25
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, "xxxxxxxx")
            };

            using (var message = new MailMessage())
            {
                message.From = fromAddress;
                foreach (string email in to)
                {
                    message.To.Add(new MailAddress(email));
                }
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = htmlBody;

                foreach (string attachmentPath in attachments)
                {
                    string mimeType = MimeTypeHelper.GetMimeType(attachmentPath);
                    Attachment attachment = new Attachment(attachmentPath, mimeType);
                    message.Attachments.Add(attachment);
                }

                smtp.Send(message);
            }
        }
    }


    public static class FileHelper
    {
        public static string ReadEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }


        public static Icon GetEmbeddedIcon(string resourceName, int size = 32)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (Bitmap bitmap = new Bitmap(stream))
            {
                Bitmap resized = new Bitmap(bitmap, new Size(size, size));
                return Icon.FromHandle(resized.GetHicon());
            }
        }

    }


    public static class AssemblyInfoHelper
    {
        public static string Title { 
            get {
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


    public static class Logger
    {
        public static readonly string BaseName =
            Assembly.GetExecutingAssembly().GetName().Name;
        private const long MaxSize = 2 * 1024 * 1024; // 2 MB
        private const int MaxBackups = 5;

        //TODO:NOTE: Ensure each thread logs to a different context to avoid race conditions.
        public static void WriteLog(string message, string context = "CORE")
        {
            try
            {
                string path = $"{BaseName}_{context}_log.txt";
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine($"{DateTime.Now:u} [{context.ToUpper()}] {message}");
                }
                RotateIfNeeded(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logging failed for context '{context}': {ex.Message}");
            }
        }

        private static void RotateIfNeeded(string path)
        {
            if (!File.Exists(path)) return;
            FileInfo fi = new FileInfo(path);
            if (fi.Length < MaxSize) return;

            try
            {
                string oldest = path.Replace(".txt", $".bak.{MaxBackups}.txt");
                if (File.Exists(oldest)) File.Delete(oldest);

                for (int i = MaxBackups - 1; i >= 1; i--)
                {
                    string src = path.Replace(".txt", $".bak.{i}.txt");
                    string dst = path.Replace(".txt", $".bak.{i + 1}.txt");
                    if (File.Exists(src)) File.Move(src, dst);
                }

                string bak1 = path.Replace(".txt", ".bak.1.txt");
                File.Move(path, bak1);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Log rotation failed for {path}: {ex.Message}");
            }
        }


        public static void CleanUp()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var allLogs = Directory.GetFiles(baseDir, $"{BaseName}_*_log.txt");
            foreach (string logPath in allLogs)
            {
                RotateIfNeeded(logPath);
            }
        }
    }

    internal static class MimeTypeHelper
    {
        private static readonly Dictionary<string, string> FallbackMimeTypes = CreateMimeTypeMap();

        private static Dictionary<string, string> CreateMimeTypeMap()
        {

            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { ".txt", "text/plain" },
                { ".pdf", "application/pdf" },
                { ".doc", "application/msword" },
                { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                { ".ppt", "application/vnd.ms-powerpoint" },
                { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
                { ".csv", "text/csv" },
                { ".rtf", "application/rtf" },
                { ".odt", "application/vnd.oasis.opendocument.text" },
                { ".ods", "application/vnd.oasis.opendocument.spreadsheet" },
                { ".odp", "application/vnd.oasis.opendocument.presentation" },

                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".png", "image/png" },
                { ".gif", "image/gif" },
                { ".bmp", "image/bmp" },
                { ".webp", "image/webp" },
                { ".svg", "image/svg+xml" },

                { ".zip", "application/zip" },
                { ".rar", "application/x-rar-compressed" },
                { ".7z", "application/x-7z-compressed" },
                { ".tar", "application/x-tar" },
                { ".gz", "application/gzip" },

                { ".html", "text/html" },
                { ".htm", "text/html" },
                { ".xml", "application/xml" },
                { ".json", "application/json" },
                { ".yaml", "application/x-yaml" },
                { ".yml", "application/x-yaml" },

                { ".mp3", "audio/mpeg" },
                { ".wav", "audio/wav" },
                { ".ogg", "audio/ogg" },
                { ".flac", "audio/flac" },

                { ".mp4", "video/mp4" },
                { ".avi", "video/x-msvideo" },
                { ".mov", "video/quicktime" },
                { ".mkv", "video/x-matroska" },

                { ".js", "application/javascript" },
                { ".ts", "application/typescript" },
                { ".css", "text/css" },
                { ".scss", "text/x-scss" },

                { ".exe", "application/vnd.microsoft.portable-executable" },
                { ".msi", "application/x-msdownload" },
                { ".apk", "application/vnd.android.package-archive" },
                { ".bat", "application/x-msdos-program" },

                { ".eot", "application/vnd.ms-fontobject" },
                { ".ttf", "font/ttf" },
                { ".woff", "font/woff" },
                { ".woff2", "font/woff2" }
            };
        }

        internal static string GetMimeType(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return "application/octet-stream";

            var extension = Path.GetExtension(filePath);

            if (!string.IsNullOrEmpty(extension) && FallbackMimeTypes.TryGetValue(extension, out var fallbackType))
            {
                return fallbackType;
            }

            return "application/octet-stream"; // Default unknown
        }
    }


}

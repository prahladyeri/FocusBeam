/**
 * Helpers.cs - Utility hub for general use
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace focusbeam.Util
{
    public static class EmojiConstants
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

    public static class Core
    {
        public static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
    }

    public static class XmlConfig
    {

        public static string Get(string key)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("config.xml");
            return doc.SelectSingleNode($"/config/{key}").InnerText;
        }

        public static void Set(string key, string value)
        {
            string configPath = "config.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(configPath);

            // Find the node
            XmlNode node = doc.SelectSingleNode($"/config/{key}");

            if (node != null)
            {
                node.InnerText = value;
            }
            else
            {
                // If key doesn't exist, create it
                XmlNode root = doc.DocumentElement;
                XmlElement newElem = doc.CreateElement(key);
                newElem.InnerText = value;
                root.AppendChild(newElem);
            }

            // Save back to file
            doc.Save(configPath);
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
                .Replace("{{regards}}", $"{Core.AppName} Support")
                ;
            var fromAddress = new MailAddress("appsdepot@outlook.com", $"{Core.AppName} Support");

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
        public static readonly string Path =
            $"{Assembly.GetExecutingAssembly().GetName().Name}_log.txt";

        public static void CleanUp()
        {
            // TODO Check if log size is greater than 100MB
            // TODO If so, rename it to something else, also warn user to delete or take backup.
            const long MaxSize = 100 * 1024 * 1024; // 100 MB
            if (File.Exists(Path))
            {
                FileInfo fi = new FileInfo(Path);
                if (fi.Length > MaxSize)
                {
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string newPath = Path.Replace(".txt", $"_{timestamp}.bak.txt");
                    File.Move(Path, newPath);
                    MessageBox.Show(
                        $"The log file exceeded 100MB and was renamed to:\n\n{newPath}\n\n" +
                        $"Please consider deleting or backing it up.",
                        "Log File Cleanup",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
        }

        public static void WriteLog(string message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(Path, true))
                {
                    writer.WriteLine(String.Format("{0}: {1}", DateTime.Now, message));
                }
            }
            catch (Exception ex)
            {
                // Handle any errors related to file access or logging
                Console.WriteLine(String.Format("Logging failed: {0}", ex.Message));
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

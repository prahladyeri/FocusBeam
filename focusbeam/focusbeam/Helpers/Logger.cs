/**
 * Logger.cs
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using System;
using System.IO;
using System.Reflection;

namespace focusbeam.Helpers
{
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
}

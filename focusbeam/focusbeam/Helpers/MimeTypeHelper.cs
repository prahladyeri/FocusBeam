/**
 * MimeTypeHelper.cs
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace focusbeam.Helpers
{
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

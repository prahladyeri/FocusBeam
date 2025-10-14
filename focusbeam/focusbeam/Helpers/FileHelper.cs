/**
 * FileHelper.cs
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using System.Drawing;
using System.IO;
using System.Reflection;

namespace focusbeam.Helpers
{
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

}

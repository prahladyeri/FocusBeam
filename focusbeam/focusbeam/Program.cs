/**
 * Program.cs - Main entry point and other utilities
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using focusbeam.Helpers;
using System;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace focusbeam
{
    static class Program
    {
        public static AppSettings Settings;
        private static Mutex mutex = null;

        public static void StyleGrid(DataGridView dgv) {
            dgv.RowHeadersVisible = false;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font(
                dgv.Font.FontFamily,
                dgv.Font.Size,
                FontStyle.Bold
            );
        }

        [STAThread]
        static void Main()
        {
            const string mutexName = "Global\\FocusBeamMutex";
            bool createdNew;
            mutex = new Mutex(true, mutexName, out createdNew);
            if (!createdNew)
            {
                MessageBox.Show("Another instance of the application is already running.", "Instance Already Running", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Settings = AppSettings.Load();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
